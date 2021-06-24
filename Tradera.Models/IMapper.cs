using System.Collections.Generic;
using Tradera.Models.ServiceResolvers;

namespace Tradera.Models
{
    public interface IMapper
    {
        public ExchangeName Name { get; }
        public List<ExchangeTicker> Process(string json);

    }
}