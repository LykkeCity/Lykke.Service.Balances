using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Logs;
using Lykke.Service.Wallets.Core;
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
        }

        //  builder.RegisterInstance<IClientAccountsRepository>(
        //    AzureRepoFactories.CreateTradersRepository(_settings.Db.ClientPersonalInfoConnString, log)
        //).SingleInstance();

    }
}
