using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.Balances.RabbitSubscribers;
using Lykke.Job.Balances.Settings;
using Lykke.Service.Balances.AzureRepositories;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.Core.Settings;
using Lykke.Service.Balances.Services;
using Lykke.Service.Balances.Services.Wallet;
using Lykke.Service.Balancess.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Job.Balances.Modules
{
    public class JobModule : Module
    {
        private readonly BalancesSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettings;
        private readonly ILog _log;

        public JobModule(BalancesSettings settings, IReloadingManager<DbSettings> dbSettings, ILog log)
        {
            _settings = settings;
            _dbSettings = dbSettings;
            _log = log;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance(_dbSettings)
                .SingleInstance();
            
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();
            
            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            
            builder.RegisterType<WalletsManager>()
                .As<IWalletsManager>()
                .WithParameter(TypedParameter.From(_settings.BalanceCache.Expiration));

            builder.Register(c => new RedisCache(new RedisCacheOptions
                {
                    Configuration = _settings.BalanceCache.Configuration,
                    InstanceName = _settings.BalanceCache.Instance
                }))
                .As<IDistributedCache>()
                .SingleInstance();

            builder.RegisterType<WalletsRepository>()
                .As<IWalletsRepository>().SingleInstance();
            
            builder.Register(kernel =>
                AzureTableStorage<WalletEntity>.Create(
                    _dbSettings.ConnectionString(x => x.BalancesConnString), "Balances",
                    kernel.Resolve<ILog>())).SingleInstance();

            builder.RegisterType<BalanceUpdateRabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.BalanceRabbit.ConnectionString));

            builder.RegisterType<ClientAuthenticatedRabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.AuthRabbit.ConnectionString));
        }
    }
}
