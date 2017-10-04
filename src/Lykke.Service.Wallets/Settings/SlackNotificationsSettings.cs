using Lykke.AzureQueueIntegration;

namespace Lykke.Service.Wallets.Settings
{
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}