using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.Balances.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public BalancesSettings BalancesJob { get; set; }

        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
    }

    public class AssetsServiceClientSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
