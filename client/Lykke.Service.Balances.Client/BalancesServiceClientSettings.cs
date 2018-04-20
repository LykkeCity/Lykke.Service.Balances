using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Balances.Client
{
    public class BalancesServiceClientSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
