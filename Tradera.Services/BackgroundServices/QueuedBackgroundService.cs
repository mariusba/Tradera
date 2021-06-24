using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Tradera.Models;

namespace Tradera.Services.BackgroundServices
{
    public class QueuedBackgroundService : IQueuedBackgroundService
    {
        private readonly BackgroundJobs _backgroundJobs;
        private readonly IBackgroundWorkerManager _manager;
        public QueuedBackgroundService(BackgroundJobs backgroundJobs, IBackgroundWorkerManager manager)
        {
            _manager = manager;
            _backgroundJobs = backgroundJobs;
        }
        public async Task StartAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_backgroundJobs.BackgroundTasks.TryDequeue(out var identifier))
                {
                    if (!_backgroundJobs.RunningTasks.ContainsKey(identifier))
                    {
                        var cts = new CancellationTokenSource();
                        try
                        {
                            await _manager.StartProcessor(identifier, cts.Token);
                            _backgroundJobs.RunningTasks.Add(identifier, cts);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("error starting processor {ex}", ex);
                        }

                    }
                }
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        public Task StopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_backgroundJobs.StoppableBackgroundTasks.TryDequeue(out var identifier))
                {
                    if (_backgroundJobs.RunningTasks.TryGetValue(identifier, out var cts))
                    {
                        cts.Cancel();
                        _manager.StopProcessor(identifier, cts.Token);
                        _backgroundJobs.RunningTasks.Remove(identifier);
                    }
                }
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            return Task.CompletedTask;
        }

        public Task Enqueue(ProcessorIdentifier identifier, bool starting = true)
        {
            if (starting)
            {
                _backgroundJobs.BackgroundTasks.Enqueue(identifier);
            }
            else
            {
                _backgroundJobs.StoppableBackgroundTasks.Enqueue(identifier);
            }
            return Task.CompletedTask;
        }
    }
}