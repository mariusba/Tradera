using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tradera.Contract;
using Tradera.Models;
using Tradera.Models.ServiceResolvers;
using Tradera.Services;
using static System.Enum;

namespace Tradera.Api.Controllers
{
    [ApiController]
    [Route("prices")]
    public class PriceController : ControllerBase
    {
        private readonly IPriceService _service;

        public PriceController(IPriceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PricesResponse>> Start([FromQuery] string pair,
            [FromQuery] string exchange = "Binance")
        {
            if (TryParse<ExchangeName>(exchange, out var exchangeEnum))
                return await _service.GetPrices(new ProcessorIdentifier(exchangeEnum, pair));
            return BadRequest("unknown exchange");
        }
    }
}