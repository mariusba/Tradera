using System.Threading;
using System.Threading.Tasks;
using Tradera.Contract;
using Tradera.Models;

namespace Tradera.Services.BackgroundServices
{
    public interface IBackgroundWorkerManager
    {
        Task StartProcessor(ProcessorIdentifier identifier, CancellationToken ct);
        Task StopProcessor(ProcessorIdentifier identifier, CancellationToken ct);
        Task<PricesResponse> GetPrice(ProcessorIdentifier identifier);
    }
}