using Common.Log;
using Lykke.Service.Balances.Client.Models;
using Lykke.Service.Balances.Client.ResponseModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Service.Balances.AutorestClient;
using Lykke.Service.Balances.AutorestClient.Models;

namespace Lykke.Service.Balances.Client
{
    public class BalancesClient : IBalancesClient, IDisposable
    {
        private readonly ILog _log;
        private IBalancesAPI _service;

        public BalancesClient(string serviceUrl, ILog log)
        {
            _service = new BalancesAPI(new Uri(serviceUrl));
            _log = log;
        }

        public async Task<IEnumerable<ClientBalanceResponseModel>> GetClientBalances(string clientId)
        {
            var response = _service.GetClientBalances(clientId);

            var error = response as ErrorResponse;

            if (response is IList<ClientBalanceResponseModel> result)
            {
                return result;
            }

            if (error != null)
            {
                await _log.WriteErrorAsync(nameof(BalancesClient), nameof(GetClientBalances),
                    $"clientId = {clientId}, message = {error.ErrorMessage}", null, DateTime.UtcNow);
            }

            return null;
        }

        public async Task<ClientBalanceModel> GetClientBalanceByAssetId(
            AutorestClient.Models.ClientBalanceByAssetIdModel model)
        {
            var response = _service.GetClientBalancesByAssetId(new AutorestClient.Models.ClientBalanceByAssetIdModel
            {
                ClientId = model.ClientId,
                AssetId = model.AssetId
            });

            var error = response as ErrorResponse;

            if (response is ClientBalanceResponseModel result)
            {
                return ClientBalanceModel.Create(result);
            }

            if (error != null)
            {
                await _log.WriteErrorAsync(nameof(BalancesClient), nameof(GetClientBalanceByAssetId),
                    $"model = {model.ToJson()}", null, DateTime.UtcNow);

                return ClientBalanceModel.Create(new ClientBalanceResponseModel {ErrorMessage = error.ErrorMessage});
            }

            return null;
        }

        public async Task<WalletCredentialsModel> GetWalletCredential(string clientId)
        {
            var response = _service.GetWalletsCredentials(clientId);

            var error = response as ErrorResponse;

            if (response is IWalletCredentials result)
            {
                return WalletCredentialsModel.Create(result);
            }

            if (error != null)
            {
                await _log.WriteErrorAsync(nameof(BalancesClient), nameof(GetWalletCredential),
                    $"clientId = {clientId}", null, DateTime.UtcNow);

                return new WalletCredentialsModel { ErrorMessage = error.ErrorMessage };
            }

            return null;
        }

        public async Task<WalletCredentialsHistoryModel> GetWalletCredentialHistory(string clientId)
        {
            var response = _service.GetWalletsCredentialsHistory(clientId);

            var error = response as ErrorResponse;

            if (response is IList<string> result)
            {
                return new WalletCredentialsHistoryModel {WalletsCredentialHistory = result};
            }

            if (error != null)
            {
                await _log.WriteErrorAsync(nameof(BalancesClient), nameof(GetWalletCredentialHistory),
                    $"clientId = {clientId}", null, DateTime.UtcNow);

                return new WalletCredentialsHistoryModel { ErrorMessage = error.ErrorMessage };
            }

            return null;
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