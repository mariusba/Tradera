namespace Tradera.Models
{
    public class ExchangeTicker
    {
        public string Type { get; init; }
        public long EventTime { get; init; }
        public string Symbol { get; init; }
        public decimal Price { get; init; }
        public string Quantity { get; init; }
        public string FirstTradeId { get; init; }
        public bool BuyerIsMarketMaker { get; init; }
        public bool Ignore { get; init; }
    }
}