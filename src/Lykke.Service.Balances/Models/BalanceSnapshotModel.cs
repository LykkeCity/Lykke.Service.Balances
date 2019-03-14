using System;

namespace Lykke.Service.Balances.Models
{
    public class BalanceSnapshotModel
    {
        public string WalletId { get; set; }
        public string AssetId { get; set; }
        public decimal Balance { get; set; }
        public decimal Reserved { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
