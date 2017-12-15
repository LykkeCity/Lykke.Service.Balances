using JetBrains.Annotations;

namespace Lykke.Service.Balances.Settings
{
    [UsedImplicitly]
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
    }
}