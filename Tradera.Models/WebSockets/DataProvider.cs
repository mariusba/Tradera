using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Tradera.Models.ServiceResolvers;

namespace Tradera.Models.WebSockets
{
    public class DataProvider
    {
        private readonly IEnumerable<IWebSocketConfigurator> _serviceResolver;

        public DataProvider(IEnumerable<IWebSocketConfigurator> services)
        {
            _serviceResolver = services;
        }

        public int GetChunkSize(ExchangeName provider)
        {
            return _serviceResolver.First(p => p.Id == provider).ChunkSize;
        }

        public async Task<ClientWebSocket> GetDataProviderAsync(ExchangeName provider, string pair, CancellationToken ct)
        {
            try
            {
                var webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(_serviceResolver.First(p => p.Id == provider).GetUri(pair), ct);

                return webSocket;
            }
            catch (Exception ex)
            {
                Log.Error("Error creating websocket {ex}", ex);
                return null;
            }
        }
        
    }
}