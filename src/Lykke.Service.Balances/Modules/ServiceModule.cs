using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.Balances.AzureRepositories;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.AzureRepositories.Wallets;
using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Core.Wallets;
using Lykke.Service.Balances.Services;
using Lykke.Service.Balances.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.Balances.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<BalancesSettings> _settings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<BalancesSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance(_settings)
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<WalletsRepository>()
                .As<IWalletsRepository>().SingleInstance();

            builder.RegisterType<WalletCredentialsRepository>()
                .As<IWalletCredentialsRepository>().SingleInstance();

            builder.RegisterType<WalletCredentialsHistoryRepository>()
                .As<IWalletCredentialsHistoryRepository>().SingleInstance();

            builder.Register(kernel =>
                AzureTableStorage<WalletEntity>.Create(
                    _settings.ConnectionString(x => x.Db.BalancesConnString), "Accounts",
                    kernel.Resolve<ILog>())).SingleInstance();

            builder.Register(kernel => AzureTableStorage<WalletCredentialsEntity>.Create(
                _settings.ConnectionString(x => x.Db.ClientPersonalInfoConnString), "WalletCredentials",
                kernel.Resolve<ILog>())).SingleInstance();

            builder.Register(kernel =>
                AzureTableStorage<WalletCredentialsHistoryRecord>.Create(
                    _settings.ConnectionString(x => x.Db.ClientPersonalInfoConnString), "WalletCredentialsHistory",
                    kernel.Resolve<ILog>())).SingleInstance();
        }
    }
}