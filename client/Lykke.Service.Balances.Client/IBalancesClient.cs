using Lykke.Service.Balances.Client.Models;
using Lykke.Service.Balances.Client.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.AutorestClient.Models;

namespace Lykke.Service.Balances.Client
{
    public interface IBalancesClient
    {
        Task<IEnumerable<ClientBalanceResponseModel>> GetClientBalances(string clientId);
        Task<ClientBalanceModel> GetClientBalanceByAssetId(AutorestClient.Models.ClientBalanceByAssetIdModel model);
        Task<WalletCredentialsModel> GetWalletCredential(string clientId);
        Task<WalletCredentialsHistoryModel> GetWalletCredentialHistory(string clientId);
        Task<TotalBalancesResponseModel> GetTotalBalances();
    }
}
