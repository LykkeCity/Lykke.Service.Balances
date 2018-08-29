using Lykke.Common.Chaos;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Balances.Settings
{
    public class CqrsSettings
    {
        [AmqpCheck]
        public string RabbitConnectionString { get; set; }
        [Optional]
        public ChaosSettings ChaosKitty { get; set; }
    }
}