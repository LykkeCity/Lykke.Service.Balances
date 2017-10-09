using Lykke.AzureQueueIntegration;

namespace Lykke.Service.Balances.Settings
{
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}