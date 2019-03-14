using Autofac;
using Lykke.Service.Balances.Settings;
using Lykke.SettingsReader;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Lykke.Service.Balances.Modules
{
    public class MongoDbModule : Module
    {
        private readonly BalanceSnapshotsSettings _settings;

        public MongoDbModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue.BalancesService.BalanceSnapshots;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var mongoUrl = new MongoUrl(_settings.ConnectionString);
            ConventionRegistry.Register("Ignore extra", new ConventionPack { new IgnoreExtraElementsConvention(true) }, x => true);

            var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
            builder.RegisterInstance(database);
        }
    }
}
