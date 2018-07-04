using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.Balances.AzureRepositories;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.AzureRepositories.Wallets;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.Core.Settings;
using Lykke.Service.Balances.Services.Wallet;
using Lykke.Service.Balances.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Lykke.Service.Balances.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly IReloadingManager<DbSettings> _dbSettings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<AppSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _dbSettings = _appSettings.Nested(x => x.BalancesService.Db);
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WalletsManager>()
                .As<IWalletsManager>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BalancesService.BalanceCache.Expiration));

            builder.Register(c => new RedisCache(new RedisCacheOptions
                {
                    Configuration = _appSettings.CurrentValue.BalancesService.BalanceCache.Configuration,
                    InstanceName = _appSettings.CurrentValue.BalancesService.BalanceCache.Instance
                }))
                .As<IDistributedCache>()
                .SingleInstance();
            
            builder.RegisterInstance(
                new WalletsRepository(AzureTableStorage<WalletEntity>.Create(
                    _dbSettings.ConnectionString(x => x.BalancesConnString), "Balances", _log))
            ).As<IWalletsRepository>().SingleInstance();
            
            builder.RegisterInstance(
                new WalletCredentialsRepository(AzureTableStorage<WalletCredentialsEntity>.Create(
                    _dbSettings.ConnectionString(x => x.ClientPersonalInfoConnString), "WalletCredentials", _log))
            ).As<IWalletCredentialsRepository>().SingleInstance();
            
            builder.RegisterInstance(
                new WalletCredentialsHistoryRepository(AzureTableStorage<WalletCredentialsHistoryRecord>.Create(
                    _dbSettings.ConnectionString(x => x.ClientPersonalInfoConnString), "WalletCredentialsHistory", _log))
            ).As<IWalletCredentialsHistoryRepository>().SingleInstance();
        }
    }
}
