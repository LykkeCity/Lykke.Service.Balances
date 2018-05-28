using Lykke.AzureQueueIntegration;

namespace Lykke.Service.Balances.Core.Settings
{
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}
