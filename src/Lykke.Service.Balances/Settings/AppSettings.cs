using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.Balances.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public BalancesSettings BalancesService { get; set; }
    }
}
