using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Tradera.Binance;
using Tradera.Models.ExchangeWrapper;
using Tradera.Models.ServiceResolvers;
using Tradera.Models.WebSockets;
using System.Reactive.Linq;
using Serilog;
using Tradera.Contract;
using Tradera.Models;
using Observable = Tradera.Models.WebSockets.Observable;

namespace Tradera.Services.BackgroundServices
{
    public class BackgroundWorkerManager : IBackgroundWorkerManager
    {
        private readonly DataProvider _provider;
        private readonly Dictionary<ProcessorIdentifier, (DataProcessor processor, List<IDisposable> disposable)> _processors = new();
        private readonly INotificationsService _notificationsService;
        private readonly IExchangeAgent _exchangeAgent;
        private readonly IEnumerable<IMapper> _mappers;
        
        public BackgroundWorkerManager(DataProvider provider, INotificationsService notificationsService,
            IExchangeAgent exchangeAgent, IEnumerable<IMapper> mappers)
        {
            _provider = provider;
            _notificationsService = notificationsService;
            _exchangeAgent = exchangeAgent;
            _mappers = mappers;
        }
        public async Task StartProcessor(ProcessorIdentifier identifier, CancellationToken ct)
        { 
            
            _exchangeAgent.AddObservable(new Observable(_provider, ExchangeName.Binance, identifier.Pair), identifier);
            var disposables = new List<IDisposable>();
            var observable = _exchangeAgent.GetObservable<string>(identifier);
            var wrapper = new ExchangeWrapper(
                _mappers.First(m => m.Name == ExchangeName.Binance));
            var wrapperDisposable = observable.Window(TimeSpan.FromTicks(2)).Subscribe(wrapper);
            var processor = new DataProcessor(_notificationsService);
            disposables.Add(wrapperDisposable);
            var processorDisposable = wrapper.Subscribe(k => processor.AddEntry(k), onCompleted: () => Log.Information("closeed"));

            disposables.Add(processorDisposable);
            _processors.Add(identifier, (processor, disposables));
            Log.Information("{exchange} {pair} processor started", identifier.Name, identifier.Pair);
                
        }

        public Task StopProcessor(ProcessorIdentifier identifier, CancellationToken ct)
        {
           if( _processors.TryGetValue(identifier, out var key))
           {
               foreach (var disposable in key.disposable)
               {
                   disposable.Dispose();
                   
               }
           }
           _processors.Remove(identifier);
           return Task.CompletedTask;
        }


        public async Task<PricesResponse> GetPrice(ProcessorIdentifier identifier)
        {
            if (_processors.TryGetValue(identifier, out var processor))
            {
                return await processor.processor.GetPrice();
            }

            return null;
        }
    }
}