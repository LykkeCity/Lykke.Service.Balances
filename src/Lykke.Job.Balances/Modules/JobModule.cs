using System;
using Autofac;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.Balances.RabbitSubscribers;
using Lykke.Job.Balances.Settings;
using Lykke.Sdk;
using Lykke.Service.Assets.Client;
using Lykke.Service.Balances.AzureRepositories;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.Core.Settings;
using Lykke.Service.Balances.Services;
using Lykke.Service.Balances.Services.Wallet;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Job.Balances.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly IReloadingManager<DbSettings> _dbSettings;

        public JobModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _dbSettings = appSettings.Nested(x => x.BalancesJob.Db);
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settings = _appSettings.CurrentValue;

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<CachedWalletsRepository>()
                .As<ICachedWalletsRepository>()
                .WithParameter(TypedParameter.From(settings.BalancesJob.BalanceCache.Expiration));

            builder.Register(c => new RedisCache(new RedisCacheOptions
            {
                Configuration = settings.BalancesJob.BalanceCache.Configuration,
                InstanceName = settings.BalancesJob.BalanceCache.Instance
            }))
                .As<IDistributedCache>()
                .SingleInstance();

            builder.RegisterAssetsClient(AssetServiceSettings.Create(new Uri(settings.AssetsServiceClient.ServiceUrl), TimeSpan.FromMinutes(5)));

            builder.Register(ctx =>
                new WalletsRepository(AzureTableStorage<WalletEntity>.Create(
                    _dbSettings.ConnectionString(x => x.BalancesConnString), "Balances", ctx.Resolve<ILogFactory>()))
            ).As<IWalletsRepository>().SingleInstance();

            builder.RegisterType<BalanceUpdateRabbitSubscriber>()
                .As<IStartStop>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(settings.BalancesJob.MatchingEngineRabbit));

            builder.RegisterType<ClientAuthenticatedRabbitSubscriber>()
                .As<IStartStop>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(settings.BalancesJob.AuthRabbit));

            builder.RegisterType<TotalBalanceCacheUpdater>()
                .As<IStartStop>()
                .SingleInstance();
        }
    }
}
