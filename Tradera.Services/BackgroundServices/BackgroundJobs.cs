using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Tradera.Models;

namespace Tradera.Services.BackgroundServices
{
    public class BackgroundJobs
    {
        public ConcurrentQueue<ProcessorIdentifier> BackgroundTasks { get; set; } = new();
        public ConcurrentQueue<ProcessorIdentifier> StoppableBackgroundTasks { get; set; } = new();
        public Dictionary<ProcessorIdentifier, CancellationTokenSource> RunningTasks { get; set; } = new();
    }
}