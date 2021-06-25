using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tradera.Contract;
using Tradera.Models;
using Tradera.Models.ServiceResolvers;
using Tradera.Services;

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
        public async Task<PricesResponse> Start([FromQuery] string exchange, [FromQuery] string pair)
        {
            return await _service.GetPrices(new ProcessorIdentifier(ExchangeName.Binance, pair));
        }
    }
}