
using Lykke.Service.Wallets.Strings;

namespace Lykke.Service.Wallets.Models.ClientBalances
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

        public static ClientBalanceResponseModel NoResultFound()
        {
            return new ClientBalanceResponseModel
            {
                ErrorMessage = Phrases.ClientBalanceNotFound
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
