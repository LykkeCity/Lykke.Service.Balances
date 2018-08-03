using Common.Log;
using Lykke.Service.Balances.Models.ClientBalances;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.Balances.Core.Services.Wallets;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Balances.Controllers
{
    [Route("api/[controller]")]
    public class WalletsClientBalancesController : Controller
    {
        private readonly ICachedWalletsRepository _cachedWalletsRepository;
        private readonly ILog _log;

        public WalletsClientBalancesController(ICachedWalletsRepository cachedWalletsRepository, ILogFactory logFactory)
        {
            _cachedWalletsRepository = cachedWalletsRepository;
            _log = logFactory.CreateLog(this);
        }

        [HttpGet]
        [Route("{clientId}")]
        [SwaggerOperation("GetClientBalances")]
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceResponseModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
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
                _log.Error(nameof(GetClientBalances), ex, $"clientId = {clientId}");

                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ErrorResponse.Create("Error occured while getting client balances"));
            }
        }

        [HttpGet]
        [Route("{clientId}/{assetId}")]
        [SwaggerOperation("GetClientBalancesByAssetId")]
        [ProducesResponseType(typeof(ClientBalanceResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
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
                _log.Error(nameof(GetClientBalancesByAssetId), ex, $"clientId = {clientId}, assetId = {assetId}");

                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ErrorResponse.Create("Error occured while getting client balance by asset"));
            }
        }

        [HttpGet]
        [Route("totalBalances")]
        [SwaggerOperation("GetTotalBalances")]
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceResponseModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
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
                _log.Error(nameof(GetTotalBalances), ex);

                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ErrorResponse.Create("Error occured while getting total balances"));
            }
        }

        [HttpPost]
        [Route("totalBalance/{assetId}")]
        [SwaggerOperation("GetTotalBalance")]
        [ProducesResponseType(typeof(ClientBalanceResponseModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTotalBalance(string assetId)
        {
            try
            {
                var balance = await _cachedWalletsRepository.GetTotalBalanceAsync(assetId);

                if (balance == null)
                    return NotFound();

                return Ok(ClientBalanceResponseModel.Create(balance));
            }
            catch (Exception ex)
            {
                _log.WriteError(nameof(WalletsClientBalancesController), nameof(GetTotalBalances), ex);

                return StatusCode((int)HttpStatusCode.InternalServerError, ErrorResponse.Create("Error occured while getting total balances"));
            }
        }

    }
}
