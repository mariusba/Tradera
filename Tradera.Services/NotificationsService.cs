using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Tradera.Models;

namespace Tradera.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly decimal _threshold = 0.1m;
        private decimal lastHighest;
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
            if (highestAmount.Price > lastHighest * (1 + _threshold / 100))
            {
                lastHighest = highestAmount.Price;
                return highestAmount;
            }

            return null;
        }

        private static void Notify(ExchangeTicker data)
        {
            Log.Information("Event triggered with data {price}",data.Price.ToString(CultureInfo.InvariantCulture));
        }
    }
}