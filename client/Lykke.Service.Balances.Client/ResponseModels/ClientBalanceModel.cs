using Lykke.Service.Balances.AutorestClient.Models;

namespace Lykke.Service.Balances.Client.ResponseModels
{
    public class ClientBalanceModel
    {
        public string AssetId { get; set; }
        public decimal Balance { get; set; }
        public decimal Reserved { get; set; }
        public string ErrorMessage { get; set; }

        public static ClientBalanceModel Create(ClientBalanceResponseModel src)
        {
            return new ClientBalanceModel
            {
                AssetId = src.AssetId,
                Balance = src.Balance,
                Reserved = src.Reserved
            };
        }
    }
}
