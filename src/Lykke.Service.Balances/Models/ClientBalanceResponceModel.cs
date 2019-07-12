using System;
using Lykke.Service.Balances.Core.Domain;

namespace Lykke.Service.Balances.Models
{
    public class ClientBalanceModel
    {
        // todo: add WalletId
        //public string WalletId { get; set; }
        public string AssetId { get; set; }
        public decimal Balance { get; set; }
        public decimal Reserved { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static ClientBalanceModel Create(IWallet src)
        {
            return new ClientBalanceModel
            {
                AssetId = src.AssetId,
                Balance = src.Balance,
                Reserved = src.Reserved,
                UpdatedAt = src.UpdatedAt
            };
        }
    }
}
