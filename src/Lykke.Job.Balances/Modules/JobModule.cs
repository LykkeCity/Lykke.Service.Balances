using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.Balances.RabbitSubscribers;
using Lykke.Job.Balances.Settings;
using Lykke.Service.Balances.AzureRepositories;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.Core.Settings;
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
        private readonly ILog _log;

        public JobModule(IReloadingManager<AppSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _dbSettings = appSettings.Nested(x => x.BalancesJob.Db);
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CachedCachedWalletsRepository>()
                .As<ICachedWalletsRepository>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BalancesJob.BalanceCache.Expiration));

            builder.Register(c => new RedisCache(new RedisCacheOptions
            {
                Configuration = _appSettings.CurrentValue.BalancesJob.BalanceCache.Configuration,
                InstanceName = _appSettings.CurrentValue.BalancesJob.BalanceCache.Instance
            }))
                .As<IDistributedCache>()
                .SingleInstance();

            builder.RegisterInstance(
                new WalletsRepository(AzureTableStorage<WalletEntity>.Create(
                    _dbSettings.ConnectionString(x => x.BalancesConnString), "Balances", _log))
            ).As<IWalletsRepository>().SingleInstance();

            builder.RegisterType<BalanceUpdateRabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BalancesJob.MatchingEngineRabbit));

            builder.RegisterType<ClientAuthenticatedRabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BalancesJob.AuthRabbit));
        }
    }
}
