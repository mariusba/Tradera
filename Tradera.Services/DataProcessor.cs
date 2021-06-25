using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Tradera.Contract;
using Tradera.Models;

namespace Tradera.Services
{
    public class DataProcessor : IDataProcessor
    {
        private readonly ConcurrentDictionary<ProcessorIdentifier,List<ExchangeTicker>> _storage = new();
        private readonly INotificationsService _notificationsService;
        private const int ItemsToStore = 100;

        public DataProcessor(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public async Task AddEntry(ExchangeTicker ticker)
        {
            if (!_storage.TryGetValue(ticker.Identifier, out var items))
            {
                items = new List<ExchangeTicker>();
               _storage.TryAdd(ticker.Identifier, items);
            }

            if (items.Count == 0 || items.Last().EventTime <= ticker.EventTime)
            {
                if (items.Count >= ItemsToStore)
                {
                    items.RemoveAt(0);
                }
                items.Add(ticker);
                await Task.Run(() => _notificationsService.DataUpdated(items));
            }
        }

        public async Task<PricesResponse> GetPrice(ProcessorIdentifier identifier)
        {
            if (!_storage.TryGetValue(identifier, out var items)) return new PricesResponse();
            if (items.Count < 2)
            {
                return new PricesResponse();
            }

            var sorted = items.OrderByDescending(i => i.Price).ToArray();
            return new PricesResponse
            {
                HighestPrice = sorted.First().Price,
                LowestPrice = sorted.Last().Price,
            };

        }

        public Task StopProcessingFor(ProcessorIdentifier identifier)
        {
           if( _storage.TryRemove(identifier, out var removed))
           {
               Log.Information("processing for {exchange} {ticker} stopped", identifier.Name, identifier.Pair);
               _notificationsService.Clear(identifier);
           }
           else

           {
               Log.Error("Unable to remove");
           }
           return Task.CompletedTask;
        }
    }
}