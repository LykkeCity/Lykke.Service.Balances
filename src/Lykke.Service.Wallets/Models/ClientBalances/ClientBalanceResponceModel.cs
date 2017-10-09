
using Lykke.Service.Balances.Strings;

namespace Lykke.Service.Balances.Models.ClientBalances
{
    public class ClientBalanceResponseModel
    {
        public string AssetId { get; set; }
        public double Balance { get; set; }
        public string ErrorMessage { get; set; }

        public static ClientBalanceResponseModel Create(Core.Wallets.IWallet src)
        {
            return new ClientBalanceResponseModel
            {
                AssetId = src.AssetId,
                Balance = src.Balance,
            };
        }

        public static ClientBalanceResponseModel CreateErrorMessage(string error)
        {
            return new ClientBalanceResponseModel
            {
                ErrorMessage = error
            };
        }
    }
}
