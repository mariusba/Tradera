using System;
using Tradera.Models.ServiceResolvers;

namespace Tradera.Models.WebSockets
{
    public interface IWebSocketConfigurator
    {
        public int ChunkSize { get; set; }
        public ExchangeName Id { get; set; }
        public Uri GetUri(string pair);
    }
}