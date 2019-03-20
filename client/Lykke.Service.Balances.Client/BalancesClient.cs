using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.Balances.AutorestClient;

namespace Lykke.Service.Balances.Client
{
    public class BalancesClient : IBalancesClient, IDisposable
    {
        private IBalancesAPI _service;

        public BalancesClient(string serviceUrl)
        {
            _service = new BalancesAPI(new Uri(serviceUrl), new HttpClient());
        }

        public Task<IList<ClientBalanceModel>> GetClientBalances(string clientId)
        {
            return _service.GetClientBalancesAsync(clientId);
        }

        public Task<ClientBalanceModel> GetClientBalanceByAssetId(ClientBalanceByAssetIdModel model)
        {
            return _service.GetClientBalancesByAssetIdAsync(model.ClientId, model.AssetId);
        }

        public Task<IList<TotalAssetBalanceModel>> GetTotalBalances()
        {
            return _service.GetTotalBalancesAsync();
        }

        public Task<TotalAssetBalanceModel> GetTotalAssetBalance(string assetId)
        {
            return _service.GetTotalAssetBalanceAsync(assetId);
        }

        public Task<IList<BalanceSnapshotShortModel>> GetWalletBalanceAtMoment(string assetId, DateTime timestamp)
        {
            return _service.GetAllWalletsBalancesAsync(assetId, timestamp);
        }

        public Task<BalanceSnapshotModel> GetWalletBalanceAtMoment(string walletId, string assetId, DateTime timestamp)
        {
            return _service.GetWalletBalanceAsync(walletId, assetId, timestamp);
        }

        public void Dispose()
        {
            if (_service == null)
                return;
            _service.Dispose();
            _service = null;
        }
    }
}
