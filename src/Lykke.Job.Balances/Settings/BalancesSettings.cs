using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Settings;

namespace Lykke.Job.Balances.Settings
{
    [UsedImplicitly]
    public class BalancesSettings
    {
        public DbSettings Db { get; set; }
        public RedisSettings BalanceCache { get; set; }
        public RabbitMqSettings BalanceRabbit { get; set; }
        public RabbitMqSettings AuthRabbit { get; set; }
    }
}
