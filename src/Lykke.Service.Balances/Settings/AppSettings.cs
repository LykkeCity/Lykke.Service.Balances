namespace Lykke.Service.Balances.Settings
{
    public class AppSettings
    {
        public BalancesSettings BalancesService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }

    public class BalancesSettings
    {
        public DbSettings Db { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string ClientPersonalInfoConnString { get; set; }
        public string BalancesConnString { get; set; }
    }
}
