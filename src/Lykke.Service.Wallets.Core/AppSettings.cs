namespace Lykke.Service.Wallets.Core
{
    public class AppSettings
    {
        public WalletsSettings WalletsService { get; set; }
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

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
