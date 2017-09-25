using Common.Log;
using Lykke.Service.Wallets.Core.Wallets;
using Lykke.Service.Wallets.Models.ClientBalances;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.Wallets.Controllers
{
    [Route("api/[controller]")]
    public class WalletsClientBalancesController : Controller
    {
        private readonly IWalletsRepository _walletsRepository;
        private readonly ILog _log;

        public WalletsClientBalancesController(IWalletsRepository walletsRepository, ILog log)
        {
            _walletsRepository = walletsRepository;
            _log = log;
        }

        [HttpGet]
        [Route("getClientBalances/{clientId}")]
        [SwaggerOperation("GetClientBalances")]
        public async Task<IEnumerable<ClientBalanceResponseModel>> GetClientBalances(string clientId)
        {
            try
            {
                var wallets = (await _walletsRepository.GetAsync(clientId)).ToArray();
                if (wallets == null || wallets.Count() == 0)
                {
                    return null;
                }
                return wallets.Select(ClientBalanceResponseModel.Create);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClientBalancesController), nameof(GetClientBalances), $"clientId = {clientId}", ex);
                return null;
            }
        }

        [HttpGet]
        [Route("getClientBalancesByAssetId")]
        [SwaggerOperation("GetClientBalancesByAssetId")]
        public async Task<ClientBalanceResponseModel> GetClientBalancesByAssetId([FromBody]ClientBalanceByAssetIdModel model)
        {
            try
            {
                var wallet = await _walletsRepository.GetAsync(model.ClientId, model.AssetId);
                if (wallet != null)
                {
                    return ClientBalanceResponseModel.Create(wallet);
                }

                return null;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClientBalancesController), nameof(GetClientBalancesByAssetId), $"clientId = {model.ClientId}, assetId = {model.AssetId}", ex);
                return ClientBalanceResponseModel.CreateErrorMessage(ex.Message);
            }
        }
    }
}
