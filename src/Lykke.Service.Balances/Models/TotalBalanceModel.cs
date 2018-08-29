using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.Models
{
    public class TotalBalanceModel
    {
        public string AssetId { get; set; }
        public decimal Balance { get; set; }

        public static TotalBalanceModel Create(IWallet src)
        {
            return new TotalBalanceModel
            {
                AssetId = src.AssetId,
                Balance = src.Balance
            };
        }
    }
}
