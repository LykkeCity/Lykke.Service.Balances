using Lykke.Service.Balances.AutorestClient.Models;

namespace Lykke.Service.Balances.Client.ResponseModels
{
    public class ClientBalanceModel
    {
        public string AssetId { get; set; }
        public double? Balance { get; set; }
        public string ErrorMessage { get; set; }

        public static ClientBalanceModel Create(ClientBalanceResponseModel src)
        {
            return new ClientBalanceModel
            {
                AssetId = src.AssetId,
                Balance = src.Balance,
                ErrorMessage = src.ErrorMessage
            };
        }
    }
}
