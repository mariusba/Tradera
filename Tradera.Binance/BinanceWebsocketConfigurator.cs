using System;
using Tradera.Models.ServiceResolvers;
using Tradera.Models.WebSockets;

namespace Tradera.Binance
{
    public class BinanceWebsocketConfigurator : IWebSocketConfigurator
    {
        public int ChunkSize { get; set; } = 64;
        public ExchangeName Id { get; set; } = ExchangeName.Binance;

        public Uri GetUri(string pair)
        {
            return new("wss://stream.binance.com:9443/ws/" + pair + "@aggTrade");
        }
    }
}