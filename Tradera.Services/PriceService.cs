using System.Threading.Tasks;
using Tradera.Contract;
using Tradera.Models;
using Tradera.Services.BackgroundServices;

namespace Tradera.Services
{
    public class PriceService : IPriceService
    {
        private readonly IBackgroundWorkerManager _manager;

        public PriceService(IBackgroundWorkerManager manager)
        {
            _manager = manager;
        }

        public async Task<PricesResponse> GetPrices(ProcessorIdentifier identifier)
        {
            return await _manager.GetPrice(identifier);
        }
    }
}