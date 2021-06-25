using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Dictionary<ProcessorIdentifier, List<IDisposable>> _processors = new();
        private readonly IExchangeAgent _exchangeAgent;
        private readonly IEnumerable<IMapper> _mappers;
        private readonly IDataProcessor _processor;
        
        public BackgroundWorkerManager(DataProvider provider,
            IExchangeAgent exchangeAgent, IEnumerable<IMapper> mappers, IDataProcessor processor)
        {
            _provider = provider;
            _exchangeAgent = exchangeAgent;
            _mappers = mappers;
            _processor = processor;
        }
        public async Task StartProcessor(ProcessorIdentifier identifier, CancellationToken ct)
        {
            _exchangeAgent.AddObservable(new Observable(_provider, identifier.Name, identifier.Pair), identifier);
            var disposables = new List<IDisposable>();
            var observable = _exchangeAgent.GetObservable<string>(identifier);
            var wrapper = new ExchangeWrapper(
                _mappers.First(m => m.Name == identifier.Name));
            
            disposables.Add(observable.Window(TimeSpan.FromMilliseconds(10)).Subscribe(wrapper));
            disposables.Add(wrapper.Subscribe(k => _processor.AddEntry(k)));

            _processors.Add(identifier, disposables);
            Log.Information("{exchange} {pair} processor started", identifier.Name, identifier.Pair);
                
        }

        public Task StopProcessor(ProcessorIdentifier identifier, CancellationToken ct)
        {
           if( _processors.TryGetValue(identifier, out var value))
           {
               foreach (var disposable in value)
               {
                   disposable.Dispose();
                   
               }
           }
           _processors.Remove(identifier);
           _processor.StopProcessingFor(identifier);
           return Task.CompletedTask;
        }


        public async Task<PricesResponse> GetPrice(ProcessorIdentifier identifier)
        { 
            return await _processor.GetPrice(identifier);

        }
    }
}