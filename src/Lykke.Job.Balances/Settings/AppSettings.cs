using Lykke.Service.Balances.Core.Settings;

namespace Lykke.Job.Balances.Settings
{
    public class AppSettings
    {
        public BalancesSettings BalancesJob { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
