using Common.Log;
using Lykke.Service.Balances.Core.Wallets;
using Lykke.Service.Balances.Models.ClientBalances;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Lykke.Service.Balances.Models;

namespace Lykke.Service.Balances.Controllers
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
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceResponseModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [SwaggerOperation("GetClientBalances")]
        public async Task<IActionResult> GetClientBalances(string clientId)
        {
            try
            {
                var wallets = (await _walletsRepository.GetAsync(clientId)).ToList();

                if (!wallets.Any()) return NotFound();

                return Ok(wallets.Select(ClientBalanceResponseModel.Create));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClientBalancesController),
                    nameof(GetClientBalances), $"clientId = {clientId}", ex);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }

        [HttpGet]
        [Route("getClientBalancesByAssetId")]
        [ProducesResponseType(typeof(ClientBalanceResponseModel),(int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [SwaggerOperation("GetClientBalancesByAssetId")]
        public async Task<IActionResult> GetClientBalancesByAssetId([FromBody]ClientBalanceByAssetIdModel model)
        {
            try
            {
                var wallet = await _walletsRepository.GetAsync(model.ClientId, model.AssetId);

                if (wallet == null)
                    return NotFound();

                return Ok(ClientBalanceResponseModel.Create(wallet));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClientBalancesController),
                    nameof(GetClientBalancesByAssetId), $"model = {model.ToJson()}", ex);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }
    }
}
