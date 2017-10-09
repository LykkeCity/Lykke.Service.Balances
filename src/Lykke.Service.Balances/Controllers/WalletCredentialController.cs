using Common.Log;
using Lykke.Service.Balances.Core.Wallets;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Threading.Tasks;

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
        public async Task<IWalletCredentials> GetWalletsCredentials(string clientId)
        {
            try
            {
                var walletCredentials = await _walletCredentialsRepository.GetAsync(clientId);               
                return walletCredentials;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletCredentialController), nameof(GetWalletsCredentials), $"clientId = {clientId}", ex);
                return null;
            }
        }
    }
}
