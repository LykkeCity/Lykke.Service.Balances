using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.Balances.Controllers
{
    [Route("api/[controller]")]
    public class IsAliveController : Controller
    {
        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerOperation("IsAlive")]
        [ProducesResponseType(typeof(IsAliveResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult Get()
        {
            try
            {
                var isAliveResponse = new IsAliveResponse
                {
                    Version =
                        Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application
                            .ApplicationVersion,
                    Env = Environment.GetEnvironmentVariable("Env")
                };

                return Ok(isAliveResponse);
            }
            catch (Exception e)
            {
                return BadRequest(ErrorResponse.Create(e.Message));
            }
        }
    }
}
