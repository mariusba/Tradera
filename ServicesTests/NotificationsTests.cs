using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
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
        private INotificationsService _service;

        [SetUp]
        public void Setup()
        {
            var options = new OptionsWrapper<NotificationOptions>(new NotificationOptions
            {
                Threshold = 0.1m
            });
            _service = new NotificationsService(options);
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
        }

        [Test]
        public async Task AssertNotificationIsSent()
        {
            using (TestCorrelator.CreateContext())
            {
                var tickers = new List<ExchangeTicker>();
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 10
                });
                _service.DataUpdated(tickers);
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 11
                });

                _service.DataUpdated(tickers);
                var messages = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.AreEqual(2, messages.Count());
            }
        }

        [Test]
        public async Task AssertNotificationIsNotSent_WhenPriceIsLower()
        {
            using (TestCorrelator.CreateContext())
            {
                var tickers = new List<ExchangeTicker>();
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 10
                });
                _service.DataUpdated(tickers);
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 9
                });

                var messages = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.AreEqual(1, messages.Count());
            }
        }

        [Test]
        public async Task AssertNotificationIsNotSent_ForDifferentTickers()
        {
            using (TestCorrelator.CreateContext())
            {
                var tickers = new List<ExchangeTicker>();
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "BTCUSDT"),
                    Price = 10
                });
                _service.DataUpdated(tickers);
                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "DOGEBTC"),
                    Price = 500
                });

                _service.DataUpdated(tickers);

                tickers.Add(new ExchangeTicker
                {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "DOGEBTC"),
                    Price = 300
                });

                _service.DataUpdated(tickers);
                var messages = TestCorrelator.GetLogEventsFromCurrentContext();
                Assert.AreEqual(2, messages.Count());
            }
        }
    }
}