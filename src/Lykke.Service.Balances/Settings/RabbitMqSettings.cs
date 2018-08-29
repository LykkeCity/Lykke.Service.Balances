using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Balances.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        [Optional]
        [AmqpCheck]
        public string AlternateConnectionString { get; set; }

        public string Exchange { get; set; }
    }
}
