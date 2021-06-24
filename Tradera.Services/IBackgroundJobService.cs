using System.Threading.Tasks;
using Tradera.Contract;

namespace Tradera.Services
{
    public interface IBackgroundJobService
    {
        public Task Start(StartTaskRequest id);
        public Task Stop(StopTaskRequest id);
    }
}