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
        private readonly List<ExchangeTicker> _items = new();
        private readonly INotificationsService _notificationsService;

        public DataProcessor(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        public async Task AddEntry(ExchangeTicker ticker)
        {
            if (_items.Count < 100)
            {
                _items.Add(ticker);
            }
            else
            {
                if (_items.Last().EventTime <= ticker.EventTime)
                {
                    _items.RemoveAt(0);
                    _items.Add(ticker);
                    await Task.Run(() => _notificationsService.DataUpdated(_items));
                }
                else
                {
                    Log.Warning("past event received");
                }
            }
        }

        public Task<PricesResponse> GetPrice()
        {
            if (_items.Count < 2)
            {
                return null;
            }
            var sorted = _items.OrderByDescending(i => i.Price).ToArray();
            return Task.FromResult(new PricesResponse
            {
                HighestPrice = sorted.First().Price,
                LowestPrice = sorted.Last().Price,
            });
        }
    }
}