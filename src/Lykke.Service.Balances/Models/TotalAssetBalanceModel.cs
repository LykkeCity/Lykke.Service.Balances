using Lykke.Service.Balances.Core.Domain;

namespace Lykke.Service.Balances.Models
{
    public class TotalAssetBalanceModel
    {
        public string AssetId { get; set; }
        public decimal Balance { get; set; }

        public static TotalAssetBalanceModel Create(TotalAssetBalance src)
        {
            return new TotalAssetBalanceModel
            {
                AssetId = src.AssetId,
                Balance = src.Balance
            };
        }
    }
}
