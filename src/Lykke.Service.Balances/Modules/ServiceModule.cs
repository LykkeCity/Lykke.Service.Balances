using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.Balances.AzureRepositories;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.AzureRepositories.Wallets;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.RabbitSubscribers;
using Lykke.Service.Balances.Services;
using Lykke.Service.Balances.Services.Wallet;
using Lykke.Service.Balances.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.Balances.Modules
{
    public class ServiceModule : Module
    {
        private readonly BalancesSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettings;
        private readonly ILog _log;
        private readonly ServiceCollection _services;

        public ServiceModule(BalancesSettings settings, IReloadingManager<DbSettings> dbSettings, ILog log)
        {
            _settings = settings;
            _dbSettings = dbSettings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance(_dbSettings)
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            RegisterWallets(builder);

            builder.Populate(_services);
        }

        private void RegisterWallets(ContainerBuilder builder)
        {
            builder.RegisterType<WalletsManager>()
                .As<IWalletsManager>()
                .WithParameter(TypedParameter.From(_settings.BalanceCache.Expiration));

            _services.AddDistributedRedisCache(options =>
            {
                options.Configuration = _settings.BalanceCache.Configuration;
                options.InstanceName = _settings.BalanceCache.Instance;
            });

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

            builder.RegisterType<WalletsRepository>()
                .As<IWalletsRepository>().SingleInstance();

            builder.RegisterType<WalletCredentialsRepository>()
                .As<IWalletCredentialsRepository>().SingleInstance();

            builder.RegisterType<WalletCredentialsHistoryRepository>()
                .As<IWalletCredentialsHistoryRepository>().SingleInstance();

            builder.Register(kernel =>
                AzureTableStorage<WalletEntity>.Create(
                    _dbSettings.ConnectionString(x => x.BalancesConnString), "Accounts",
                    kernel.Resolve<ILog>())).SingleInstance();

            builder.Register(kernel => AzureTableStorage<WalletCredentialsEntity>.Create(
                _dbSettings.ConnectionString(x => x.ClientPersonalInfoConnString), "WalletCredentials",
                kernel.Resolve<ILog>())).SingleInstance();

            builder.Register(kernel =>
                AzureTableStorage<WalletCredentialsHistoryRecord>.Create(
                    _dbSettings.ConnectionString(x => x.ClientPersonalInfoConnString), "WalletCredentialsHistory",
                    kernel.Resolve<ILog>())).SingleInstance();
        }
    }
}