using Common.Log;
using Lykke.Service.Balances.Models.ClientBalances;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.Balances.Core.Services.Wallets;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Balances.Controllers
{
    [Route("api/[controller]")]
    public class WalletsClientBalancesController : Controller
    {
        private readonly ICachedWalletsRepository _cachedWalletsRepository;
        private readonly ILog _log;

        public WalletsClientBalancesController(ICachedWalletsRepository cachedWalletsRepository, ILog log)
        {
            _cachedWalletsRepository = cachedWalletsRepository;
            _log = log;
        }

        [HttpGet]
        [Route("{clientId}")]
        [SwaggerOperation("GetClientBalances")]
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceResponseModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetClientBalances(string clientId)
        {
            try
            {
                var wallets = await _cachedWalletsRepository.GetAllAsync(clientId);
                var result = wallets.Select(ClientBalanceResponseModel.Create);
                   
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
        [Route("{clientId}/{assetId}")]
        [SwaggerOperation("GetClientBalancesByAssetId")]
        [ProducesResponseType(typeof(ClientBalanceResponseModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetClientBalancesByAssetId(string clientId, string assetId)
        {
            try
            {
                var wallet = await _cachedWalletsRepository.GetAsync(clientId, assetId);

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
        
        [HttpGet]
        [Route("totalBalances")]
        [SwaggerOperation("GetTotalBalances")]
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceResponseModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTotalBalances()
        {
            try
            {
                var balances = await _cachedWalletsRepository.GetTotalBalancesAsync();

                if (balances == null)
                    return NotFound();

                var result = balances.Select(ClientBalanceResponseModel.Create);
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletsClientBalancesController),
                    nameof(GetTotalBalances), null, ex);

                return StatusCode((int) HttpStatusCode.InternalServerError,
                    ErrorResponse.Create("Error occured while getting total balances"));
            }
        }
    }
}
