using Lykke.Service.Balances.AutorestClient;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client.ResponseModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Client
{
    public class BalancesClient : IBalancesClient, IDisposable
    {
        private const string UexpectedApiResponse = "Unexpected Balances API response";

        private IBalancesAPI _service;

        public BalancesClient(string serviceUrl)
        {
            _service = new BalancesAPI(new Uri(serviceUrl), new HttpClient());
        }

        public async Task<IEnumerable<ClientBalanceResponseModel>> GetClientBalances(string clientId)
        {
            var response = await _service.GetClientBalancesAsync(clientId);

            if (response is ErrorResponse error)
            {
                throw new Exception(error.ErrorMessage);
            }

            if (response is IList<ClientBalanceResponseModel> result)
            {
                return result;
            }

            throw new Exception(UexpectedApiResponse);
        }

        public async Task<ClientBalanceModel> GetClientBalanceByAssetId(
            AutorestClient.Models.ClientBalanceByAssetIdModel model)
        {
            var response = await _service.GetClientBalancesByAssetIdAsync(model.ClientId, model.AssetId);

            if (response == null)
            {
                return null;
            }

            if (response is ErrorResponse error)
            {
                throw new Exception(error.ErrorMessage);
            }

            if (response is ClientBalanceResponseModel result)
            {
                return ClientBalanceModel.Create(result);
            }

            throw new Exception(UexpectedApiResponse);
        }

        public async Task<IEnumerable<ClientBalanceResponseModel>> GetTotalBalances()
        {
            var response = await _service.GetTotalBalancesAsync();

            if (response is ErrorResponse error)
            {
                throw new Exception(error.ErrorMessage);
            }

            if (response is IEnumerable<ClientBalanceResponseModel> result)
            {
                return result;
            }

            throw new Exception(UexpectedApiResponse);
        }

        public async Task<ClientBalanceResponseModel> GetTotalBalance(string assetId)
        {
            var response = await _service.GetTotalBalanceAsync(assetId);

            if (response is ErrorResponse error)
                throw new Exception(error.ErrorMessage);

            if (response is ClientBalanceResponseModel result)
                return result;

            throw new Exception(UexpectedApiResponse);
        }

        public async Task<ClientBalanceResponseModel> GetTotalBalance(string assetId)
        {
            var response = await _service.GetTotalBalanceAsync(assetId);

            if (response is ErrorResponse error)
                throw new Exception(error.ErrorMessage);

            if (response is ClientBalanceResponseModel result)
                return result;

            throw new Exception(UexpectedApiResponse);
        }

        public async Task<ClientBalanceResponseModel> GetTotalBalance(string assetId)
        {
            var response = await _service.GetTotalBalanceAsync(assetId);

            if (response is ErrorResponse error)
                throw new Exception(error.ErrorMessage);

            if (response is ClientBalanceResponseModel result)
                return result;

            throw new Exception(UexpectedApiResponse);
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
