using Common.Log;
using Lykke.Service.Wallets.Client.AutorestClient;
using Lykke.Service.Wallets.Client.AutorestClient.Models;
using Lykke.Service.Wallets.Client.Models;
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

        public async Task<IEnumerable<ClientBalanceResponseModel>> GetClientBalances(string clientId)
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

        public async Task<ClientBalanceModel> GetClientBalanceByAssetId(AutorestClient.Models.ClientBalanceByAssetIdModel model)
        {
            try
            {
                return ClientBalanceModel.Create(_service.GetClientBalancesByAssetId(new AutorestClient.Models.ClientBalanceByAssetIdModel()
                {
                    ClientId = model.ClientId,
                    AssetId = model.AssetId
                }));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClient), nameof(GetClientBalances), $"clientId = {model.ClientId}, assetId = {model.AssetId}", ex);
                return ClientBalanceModel.Create(new ClientBalanceResponseModel() { ErrorMessage = ex.Message });
            }
        }

        public async Task<WalletCredentialsModel> GetWalletCredential(string clientId)
        {
            try
            {
                var walletsCredential = _service.GetWalletsCredentials(clientId);
                return WalletCredentialsModel.Create(walletsCredential);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClient), nameof(GetWalletCredential), $"clientId = {clientId}", ex);
                return new WalletCredentialsModel() { ErrorMessage = ex.Message };
            }
        }

        public async Task<WalletCredentialsHistoryModel> GetWalletCredentialHistory(string clientId)
        {
            try
            {
                var walletsCredentialHistory = _service.GetWalletsCredentialsHistory(clientId);
                return new WalletCredentialsHistoryModel() { WalletsCredentialHistory = walletsCredentialHistory };
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClient), nameof(GetWalletCredential), $"clientId = {clientId}", ex);
                return new WalletCredentialsHistoryModel() { ErrorMessage = ex.Message };
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
