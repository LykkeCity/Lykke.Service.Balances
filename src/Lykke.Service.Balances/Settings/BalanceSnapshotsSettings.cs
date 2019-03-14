using System;
using JetBrains.Annotations;

namespace Lykke.Service.Balances.Settings
{
    [UsedImplicitly]
    public class BalanceSnapshotsSettings
    {
        public string ConnectionString { get; set; }
        public TimeSpan TimeFrame { get; set; } = TimeSpan.FromDays(1);
    }
}
