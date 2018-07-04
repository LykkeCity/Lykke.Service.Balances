using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Job.Balances.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public BalancesSettings BalancesJob { get; set; }
    }
}
