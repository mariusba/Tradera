using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Tradera.Models;
using Tradera.Services.Utils;

namespace Tradera.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly decimal _threshold = 0.1m;
        private readonly Dictionary<ProcessorIdentifier, decimal> lastHighest = new();
        private readonly KeyedSemaphoresCollection _semaphoresCollection = new();
        public Task DataUpdated(IEnumerable<ExchangeTicker> updatedData)
        {
            var notifyWith = ShouldNotify(updatedData);
            if (notifyWith != null)
            {
                Notify(notifyWith);
            }
            return Task.CompletedTask;
        }

        private ExchangeTicker ShouldNotify(IEnumerable<ExchangeTicker> updatedData)
        {
            var highestAmount = updatedData.OrderByDescending(u => u.Price).First();
            var semaphore = _semaphoresCollection.GetOrCreate(highestAmount.Identifier);
            semaphore.Wait();
            if (lastHighest.TryGetValue(highestAmount.Identifier, out var prev))
            {
                if (highestAmount.Price > prev * (1 + _threshold / 100))
                {
                    lastHighest[highestAmount.Identifier] = highestAmount.Price;
                    semaphore.Release();
                    return highestAmount;
                }
            }
            else
            {
                lastHighest.Add(highestAmount.Identifier,highestAmount.Price);
                semaphore.Release();
                return highestAmount;
            }

            semaphore.Release();
            return null;
        }

        private static void Notify(ExchangeTicker data)
        {
            Log.Information("Event triggered with data {price} at time {time}",
                data.Price.ToString(CultureInfo.InvariantCulture), data.EventTime);
        }
    }
}