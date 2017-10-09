namespace Lykke.Service.Balances.Settings
{
    public class AppSettings
    {
        public WalletsSettings WalletsServiceSettings { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }

    public class WalletsSettings
    {
        public DbSettings Db { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string ClientPersonalInfoConnString { get; set; }
    }
}
