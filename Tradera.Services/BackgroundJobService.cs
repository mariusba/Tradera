using System.Threading.Tasks;
using Tradera.Contract;
using Tradera.Services.BackgroundServices;

namespace Tradera.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IQueuedBackgroundService _backgroundQueueService;

        public BackgroundJobService(IQueuedBackgroundService backgroundQueueService)
        {
            _backgroundQueueService = backgroundQueueService;
        }

        public async Task Start(StartTaskRequest request)
        {
            await _backgroundQueueService.Enqueue(request.Identifier);
        }

        public async Task Stop(StopTaskRequest request)
        {
            await _backgroundQueueService.Enqueue(request.Identifier, false);
        }
    }
}