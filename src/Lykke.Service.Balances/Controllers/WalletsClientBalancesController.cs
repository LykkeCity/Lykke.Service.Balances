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

namespace Lykke.Service.Balances.Controllers
{
    [Route("api/[controller]")]
    public class WalletsClientBalancesController : Controller
    {
        private readonly IWalletsRepository _walletsRepository;

        public WalletsClientBalancesController(IWalletsRepository walletsRepository)
        {
            _walletsRepository = walletsRepository;
        }

        [HttpGet]
        [Route("getClientBalances/{clientId}")]
        [ProducesResponseType(typeof(IEnumerable<ClientBalanceResponseModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [SwaggerOperation("GetClientBalances")]
        public async Task<IEnumerable<ClientBalanceResponseModel>> GetClientBalances(string clientId)
        {
            var wallets = await _walletsRepository.GetAsync(clientId);

            return wallets.Select(ClientBalanceResponseModel.Create);            
        }

        [HttpGet]
        [Route("getClientBalancesByAssetId")]
        [ProducesResponseType(typeof(ClientBalanceResponseModel),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [SwaggerOperation("GetClientBalancesByAssetId")]
        public async Task<IActionResult> GetClientBalancesByAssetId([FromBody]ClientBalanceByAssetIdModel model)
        {
            var wallet = await _walletsRepository.GetAsync(model.ClientId, model.AssetId);

            if (wallet == null)
                return NotFound();

            return Ok(ClientBalanceResponseModel.Create(wallet));                
        }
    }
}
