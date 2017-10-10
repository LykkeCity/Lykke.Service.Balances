using Common.Log;
using Lykke.Service.Balances.Core.Wallets;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.Balances.Models;

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
            _log = log;
        }

        [HttpGet]
        [Route("getWalletsCredentials/{clientId}")]
        [SwaggerOperation("GetWalletsCredentials")]
        [ProducesResponseType(typeof(IWalletCredentials), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetWalletsCredentials(string clientId)
        {
            try
            {
                var walletCredentials = await _walletCredentialsRepository.GetAsync(clientId);
                
                return Ok(walletCredentials);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletCredentialController), nameof(GetWalletsCredentials), $"clientId = {clientId}", ex);

                return BadRequest(ErrorResponse.Create(ex.Message));
            }
        }
    }
}
