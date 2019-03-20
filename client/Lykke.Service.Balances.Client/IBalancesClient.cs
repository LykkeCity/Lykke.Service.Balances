using System;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Client
{
    public interface IBalancesClient
    {
        Task<IList<ClientBalanceModel>> GetClientBalances(string clientId);
        Task<ClientBalanceModel> GetClientBalanceByAssetId(ClientBalanceByAssetIdModel model);
        Task<IList<TotalAssetBalanceModel>> GetTotalBalances();
        Task<TotalAssetBalanceModel> GetTotalAssetBalance(string assetId);
        Task<IList<BalanceSnapshotShortModel>> GetWalletBalanceAtMoment(string assetId, DateTime timestamp);
        Task<BalanceSnapshotModel> GetWalletBalanceAtMoment(string walletId, string assetId, DateTime timestamp);
    }
}
