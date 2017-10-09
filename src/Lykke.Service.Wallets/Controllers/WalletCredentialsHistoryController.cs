using Common.Log;
using Lykke.Service.Balances.Core.Wallets;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<string>> GetWalletsCredentialsHistory(string clientId)
        {
            try
            {
                var walletCredentialsHistory = await _walletCredentialsHistoryRepository.GetPrevMultisigsForUser(clientId);
                return walletCredentialsHistory;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(WalletCredentialsHistoryController),
                    nameof(GetWalletsCredentialsHistory), $"clientId = {clientId}", ex);
                return null;
            }
        }
    }
}
