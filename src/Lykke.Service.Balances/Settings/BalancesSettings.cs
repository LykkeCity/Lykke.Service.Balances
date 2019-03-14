using JetBrains.Annotations;

namespace Lykke.Service.Balances.Settings
{
    [UsedImplicitly]
    public class BalancesSettings
    {
        public DbSettings Db { get; set; }
        public RedisSettings BalanceCache { get; set; }
        public RabbitMqSettings MatchingEngineRabbit { get; set; }
        public RabbitMqSettings AuthRabbit { get; set; }
        public CqrsSettings Cqrs { get; set; }
        public BalanceSnapshotsSettings BalanceSnapshots { get; set; }
    }
}
