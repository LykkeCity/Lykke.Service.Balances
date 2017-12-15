using JetBrains.Annotations;

namespace Lykke.Service.Balances.Settings
{
    [UsedImplicitly]
    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string ClientPersonalInfoConnString { get; set; }
        public string BalancesConnString { get; set; }
    }
}