using JetBrains.Annotations;

namespace Lykke.Service.Balances.Settings
{
    [UsedImplicitly]
    public class AppSettings
    {
        public BalancesSettings BalancesService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
