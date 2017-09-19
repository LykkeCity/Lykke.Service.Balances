using Common.Log;
using Lykke.Service.Wallets.Client.AutorestClient;
using Lykke.Service.Wallets.Client.AutorestClient.Models;
using Lykke.Service.Wallets.Client.ResponseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Wallets.Client
{
    public class WalletsClient : IWalletsClient, IDisposable
    {
        private readonly ILog _log;
        private WalletsService _service;

        public WalletsClient(string serviceUrl, ILog log)
        {
            _service = new WalletsService(new Uri(serviceUrl));
            _log = log;
        }

        public async Task<IList<ClientBalanceResponseModel>> GetClientBalances(string clientId)
        {
            try
            {
                return _service.GetClientBalances(clientId);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClient), nameof(GetClientBalances), $"clientId = {clientId}", ex);
                return null;
            }
        }

        public async Task<ClientBalanceModel> GetClientBalanceByAssetId(string clientId, string assetId)
        {
            try
            {
                return ClientBalanceModel.Create(_service.GetClientBalancesByAssetId(clientId, assetId));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClient), nameof(GetClientBalances), $"clientId = {clientId}", ex);
                return ClientBalanceModel.Create(new ClientBalanceResponseModel() { ErrorMessage = ex.Message });
            }
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
