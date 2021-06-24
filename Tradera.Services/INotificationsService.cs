using System.Collections.Generic;
using System.Threading.Tasks;
using Tradera.Models;

namespace Tradera.Services
{
    public interface INotificationsService
    {
        public Task DataUpdated(IEnumerable<ExchangeTicker> updatedData);
    }
}