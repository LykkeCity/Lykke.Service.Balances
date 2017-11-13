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
        [SwaggerOperation("GetClientBalances")]
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceResponseModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetClientBalances(string clientId)
        {
            try
            {
                var wallets = await _walletsRepository.GetAsync(clientId);

                var result = wallets.Any()
                    ? wallets.Select(ClientBalanceResponseModel.Create)
                    : new ClientBalanceResponseModel[0];
                   
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClientBalancesController),
                    nameof(GetClientBalances), $"clientId = {clientId}", ex);

                return StatusCode((int) HttpStatusCode.InternalServerError,
                    ErrorResponse.Create("Error occured while getting client balances"));
            }
        }

        [HttpGet]
        [Route("getClientBalancesByAssetId/{assetId}")]
        [SwaggerOperation("GetClientBalancesByAssetId")]
        [ProducesResponseType(typeof(ClientBalanceResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetClientBalancesByAssetId(string assetId, [FromQuery] string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return BadRequest(ErrorResponse.Create("ClientId can't be empty"));
            }

            try
            {
                var wallet = await _walletsRepository.GetAsync(clientId, assetId);

                if (wallet == null)
                    return NotFound();

                return Ok(ClientBalanceResponseModel.Create(wallet));
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClientBalancesController),
                    nameof(GetClientBalancesByAssetId), $"clientId = {clientId}, assetId = {assetId}", ex);

                return StatusCode((int) HttpStatusCode.InternalServerError,
                    ErrorResponse.Create("Error occured while getting client balance by asset"));
            }
        }
    }
}
