using System;
using Tradera.Models;
using Tradera.Models.ServiceResolvers;
using Tradera.Models.WebSockets;

namespace Tradera.Binance
{
    public class RandomConf : IWebSocketConfigurator
    {
        public int ChunkSize { get; set; } = 64;
        public ExchangeName Id { get; set; } = ExchangeName.Other;

        public Uri GetUri(string pair)
        {
            return new Uri("wss://stream.binance.com:9443/ws/" + pair + "@aggTrade");
        }
    }
}