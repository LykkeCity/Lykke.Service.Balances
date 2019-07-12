using Autofac;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.Service.Balances.AzureRepositories;
using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Services;
using Lykke.Service.Balances.Services.Wallet;
using Lykke.Service.Balances.Settings;
using Lykke.Service.Balances.Workflow.Handlers;
using Lykke.SettingsReader;

namespace Lykke.Service.Balances.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly IReloadingManager<DbSettings> _dbSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            _dbSettings = _appSettings.Nested(x => x.BalancesService.Db);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<TotalBalancesService>()
                .WithParameter(TypedParameter.From($"{_appSettings.CurrentValue.BalancesService.BalanceCache.Instance}:total"))
                .As<ITotalBalancesService>();

            builder.RegisterType<CachedWalletsRepository>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BalancesService.BalanceCache.Expiration))
                .WithParameter(TypedParameter.From($"{_appSettings.CurrentValue.BalancesService.BalanceCache.Instance}:balances"))
                .As<ICachedWalletsRepository>();

            builder.Register(ctx =>
                new WalletsRepository(AzureTableStorage<WalletEntity>.Create(
                    _dbSettings.ConnectionString(x => x.BalancesConnString), "Balances", ctx.Resolve<ILogFactory>()))
            ).As<IWalletsRepository>().SingleInstance();

            builder.RegisterType<BalanceUpdateRabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BalancesService.MatchingEngineRabbit));

            builder.RegisterType<ClientAuthenticatedRabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BalancesService.AuthRabbit));

        }
    }
}
