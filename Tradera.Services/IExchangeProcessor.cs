using System.Threading.Tasks;
using Tradera.Models;

namespace Tradera.Services
{
    public interface IDataProcessor
    {
        public Task AddEntry(ExchangeTicker ticker);

    }
}