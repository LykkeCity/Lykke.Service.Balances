using Common.Log;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.Log;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Balances.Controllers
{
    [Route("api/[controller]")]
    public class WalletCredentialController : Controller
    {
        private readonly IWalletCredentialsRepository _walletCredentialsRepository;
        private readonly ILog _log;

        public WalletCredentialController(
                        IWalletCredentialsRepository walletCredentialsRepository,
                        ILog log)
        {
            _walletCredentialsRepository = walletCredentialsRepository;
            _log = log.CreateComponentScope(nameof(WalletCredentialController));
        }

        [HttpGet]
        [Route("getWalletsCredentials/{clientId}")]
        [SwaggerOperation("GetWalletsCredentials")]
        [ProducesResponseType(typeof(IWalletCredentials), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetWalletsCredentials(string clientId)
        {
            try
            {
                var walletCredentials = await _walletCredentialsRepository.GetAsync(clientId);

                if (walletCredentials == null)
                    return NotFound();

                return Ok(walletCredentials);
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"clientId = {clientId}", nameof(GetWalletsCredentials));

                return StatusCode((int) HttpStatusCode.InternalServerError, ErrorResponse.Create("Error occured while getting wallet credentials"));
            }
        }
    }
}
