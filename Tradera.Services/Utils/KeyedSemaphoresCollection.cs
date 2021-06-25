using System.Collections.Concurrent;
using System.Threading;
using Tradera.Models;

namespace Tradera.Services.Utils
{
    public class KeyedSemaphoresCollection
    {
        private readonly ConcurrentDictionary<ProcessorIdentifier, SemaphoreSlim> _semaphores = new();
        public SemaphoreSlim GetOrCreate(ProcessorIdentifier identifier)
        {
            lock (_semaphores)
            {
                if (_semaphores.TryGetValue(identifier, out var semaphoreSlim))
                {
                    return semaphoreSlim;
                }

                var k = new SemaphoreSlim(1, 1);
                _semaphores.TryAdd(identifier, k);
                return k;
            }
            
        }
        
        
    }
}