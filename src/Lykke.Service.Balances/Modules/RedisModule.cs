using Autofac;
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
            System.Threading.ThreadPool.SetMinThreads(100, 100);
            var options = ConfigurationOptions.Parse(_settings.Configuration);
            options.ReconnectRetryPolicy = new ExponentialRetry(3000, 15000);

            var redis = ConnectionMultiplexer.Connect(options);

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
