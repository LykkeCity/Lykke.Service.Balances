using Common.Log;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Balances.Controllers
{
    [Route("api/[controller]")]
    public class WalletCredentialsHistoryController : Controller
    {
        private readonly IWalletCredentialsHistoryRepository _walletCredentialsHistoryRepository;
        private readonly ILog _log;

        public WalletCredentialsHistoryController(
            IWalletCredentialsHistoryRepository walletCredentialsHistoryRepository,
                        ILog log
            )
        {
            _walletCredentialsHistoryRepository = walletCredentialsHistoryRepository;
            _log = log;
        }

        [HttpGet]
        [Route("getWalletsCredentialsHistory/{clientId}")]
        [SwaggerOperation("GetWalletsCredentialsHistory")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWalletsCredentialsHistory(string clientId)
        {
            try
            {
                var walletCredentialsHistory =
                    await _walletCredentialsHistoryRepository.GetPrevMultisigsForUser(clientId);

                return Ok(walletCredentialsHistory);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletCredentialsHistoryController),
                    nameof(GetWalletsCredentialsHistory), $"clientId = {clientId}", ex);

                return StatusCode((int) HttpStatusCode.InternalServerError, ErrorResponse.Create("Error occured while getting wallet credentials history"));
            }
        }
    }
}
