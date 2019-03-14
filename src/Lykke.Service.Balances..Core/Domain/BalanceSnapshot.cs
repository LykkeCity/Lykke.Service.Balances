using System;

namespace Lykke.Service.Balances.Core.Domain
{
    public class BalanceSnapshot : IHasId
    {
        public Guid Id { get; set; }
        public string WalletId { get; set; }
        public string AssetId { get; set; }
        public decimal Balance { get; set; }
        public decimal Reserved { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
