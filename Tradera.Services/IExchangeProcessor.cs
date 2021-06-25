using System.Threading.Tasks;
using Tradera.Contract;
using Tradera.Models;

namespace Tradera.Services
{
    public interface IDataProcessor
    {
        public Task AddEntry(ExchangeTicker ticker);
        public PricesResponse GetPrice(ProcessorIdentifier identifier);
        public Task StopProcessingFor(ProcessorIdentifier identifier);

    }
}