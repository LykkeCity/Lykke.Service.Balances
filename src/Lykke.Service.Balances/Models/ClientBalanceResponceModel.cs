using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.Models
{
    public class ClientBalanceResponseModel
    {
        // todo: add WalletId
        //public string WalletId { get; set; }
        public string AssetId { get; set; }
        public decimal Balance { get; set; }
        public decimal Reserved { get; set; }

        public static ClientBalanceResponseModel Create(IWallet src)
        {
            return new ClientBalanceResponseModel
            {
                AssetId = src.AssetId,
                Balance = src.Balance,
                Reserved = src.Reserved
            };
        }
    }
}
