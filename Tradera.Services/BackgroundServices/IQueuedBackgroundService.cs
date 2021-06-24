using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tradera.Models;

namespace Tradera.Services.BackgroundServices
{
    public interface IQueuedBackgroundService : IHostedService
    {
        public Task Enqueue(ProcessorIdentifier identifier, bool starting = true);
    }
}