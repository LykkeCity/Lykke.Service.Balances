using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Logs;
using Lykke.Service.Wallets.AzureRepositories;
using Lykke.Service.Wallets.Core;
using Lykke.Service.Wallets.Core.Wallets;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.Wallets.Modules
{
    public class ServiceModule : Module
    {
        private readonly WalletsSettings _settings;
        const string appName = "Lykke.Service.Wallets";

        public ServiceModule(WalletsSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings)
                .SingleInstance();

            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            var log = new LykkeLogToAzureStorage(PlatformServices.Default.Application.ApplicationName,
                                     new LykkeLogToAzureStoragePersistenceManager(nameof(Wallets),
                                     AzureTableStorage<LogEntity>.Create(() => _settings.Db.LogsConnString, "LykkeWalletsServiceLog", null)));

            builder.RegisterInstance(log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterInstance<IWalletsRepository>(
              AzureRepoFactories.CreateWalletsRepository(_settings.Db.ClientPersonalInfoConnString, log)
          ).SingleInstance();

            builder.RegisterInstance<IWalletCredentialsRepository>(
              AzureRepoFactories.CreateWalletsCredentialsRepository(_settings.Db.ClientPersonalInfoConnString, log)
          ).SingleInstance();

            builder.RegisterInstance<IWalletCredentialsHistoryRepository>(
             AzureRepoFactories.CreateWalletCredentialsHistoryRepository(_settings.Db.ClientPersonalInfoConnString, log)
         ).SingleInstance();
        }
    }
}
