using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Client
{
    public interface IBalancesClient
    {
        Task<IEnumerable<ClientBalanceResponseModel>> GetClientBalances(string clientId);
        Task<ClientBalanceModel> GetClientBalanceByAssetId(AutorestClient.Models.ClientBalanceByAssetIdModel model);
        Task<IEnumerable<ClientBalanceResponseModel>> GetTotalBalances();
    }
}
