using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.Balances.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }
        public string Exchange { get; set; }
    }
}
