using System.Threading.Tasks;
using Tradera.Contract;
using Tradera.Models;

namespace Tradera.Services
{
    public interface IPriceService
    {
        public Task<PricesResponse> GetPrices(ProcessorIdentifier identifier);
    }
}