using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using Tradera.Models;
using Tradera.Models.ServiceResolvers;
using Tradera.Services;


namespace ServicesTests
{
    public class NotificationsTests
    {

        [SetUp]
        public void Setup()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
        }

        [Test]
        public async Task AssertNotificationIsSent()
        {
            using (TestCorrelator.CreateContext())
            {
                var notificationsService = new NotificationsService();
                var tickers = new List<ExchangeTicker>();
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 10
                });
                notificationsService.DataUpdated(tickers);
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 11
                });

                notificationsService.DataUpdated(tickers);
                var messages = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.AreEqual(2, messages.Count());
            }
        }
        
        [Test]
        public async Task AssertNotificationIsNotSent_WhenPriceIsLower()
        {
            using (TestCorrelator.CreateContext())
            {
                var notificationsService = new NotificationsService();
                var tickers = new List<ExchangeTicker>();
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 10
                });
                notificationsService.DataUpdated(tickers);
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 9
                });

                notificationsService.DataUpdated(tickers);
                var messages = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.AreEqual(1, messages.Count());
            }
        }
        
        [Test]
        public async Task AssertNotificationIsNotSent_ForDifferentTickers()
        {
            using (TestCorrelator.CreateContext())
            {
                var notificationsService = new NotificationsService();
                var tickers = new List<ExchangeTicker>();
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 10
                });
                notificationsService.DataUpdated(tickers);
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "DOGEBTC"),
                    Price = 500
                });

                notificationsService.DataUpdated(tickers);
                
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "DOGEBTC"),
                    Price = 300
                });

                notificationsService.DataUpdated(tickers);
                var messages = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.AreEqual(2, messages.Count());
            }
        }
        
    }
}