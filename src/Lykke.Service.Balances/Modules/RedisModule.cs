using Autofac;
using Lykke.Service.Balances.Core.Settings;
using Lykke.Service.Balances.Settings;
using Lykke.SettingsReader;
using StackExchange.Redis;

namespace Lykke.Service.Balances.Modules
{
    public class RedisModule : Module
    {
        private readonly RedisSettings _settings;

        public RedisModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue.BalancesService.BalanceCache;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var redis = ConnectionMultiplexer.Connect(_settings.Configuration);

            builder.RegisterInstance(redis).SingleInstance();
            builder.Register(
                c =>
                    c.Resolve<ConnectionMultiplexer>()
                        .GetServer(redis.GetEndPoints()[0]));

            builder.Register(
                c =>
                    c.Resolve<ConnectionMultiplexer>()
                        .GetDatabase());
        }
    }
}
