using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Controllers
{
    [Route("api/[controller]")]
    public class WalletsClientBalancesController : Controller
    {
        private readonly ICachedWalletsRepository _cachedWalletsRepository;
        private readonly ITotalBalancesService _totalBalancesService;

        public WalletsClientBalancesController(ICachedWalletsRepository cachedWalletsRepository, ITotalBalancesService totalBalancesService)
        {
            _cachedWalletsRepository = cachedWalletsRepository;
            _totalBalancesService = totalBalancesService;
        }

        [HttpGet]
        [Route("{clientId}")]
        [SwaggerOperation("GetClientBalances")]
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetClientBalances(string clientId)
        {
            var wallets = await _cachedWalletsRepository.GetAllAsync(clientId);
            var result = wallets.Select(ClientBalanceModel.Create);

            return Ok(result);
        }

        [HttpGet]
        [Route("{clientId}/{assetId}")]
        [SwaggerOperation("GetClientBalancesByAssetId")]
        [ProducesResponseType(typeof(ClientBalanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetClientBalancesByAssetId(string clientId, string assetId)
        {
            var wallet = await _cachedWalletsRepository.GetAsync(clientId, assetId);

            if (wallet == null)
                return NotFound();

            return Ok(ClientBalanceModel.Create(wallet));
        }

        [HttpGet]
        [Route("totalBalances")]
        [SwaggerOperation("GetTotalBalances")]
        [ProducesResponseType(typeof(IEnumerable<TotalAssetBalanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTotalBalances()
        {
            var balances = await _totalBalancesService.GetTotalBalancesAsync();

            if (balances == null)
                return NotFound();

            var result = balances.Select(TotalAssetBalanceModel.Create);
            return Ok(result);
        }

        [HttpGet]
        [Route("totalBalances/{assetId}")]
        [SwaggerOperation("GetTotalAssetBalance")]
        [ProducesResponseType(typeof(TotalAssetBalanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTotalAssetBalance(string assetId)
        {
            var balance = await _totalBalancesService.GetTotalAssetBalanceAsync(assetId);

            if (balance == null)
                return NotFound();

            return Ok(TotalAssetBalanceModel.Create(balance));
        }

        [HttpPost]
        [Route("init")]
        [SwaggerOperation("InitTotalAssetBalance")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> InitTotalAssetBalance()
        {
            await _totalBalancesService.InitAsync();

            return Ok();
        }
    }
}
