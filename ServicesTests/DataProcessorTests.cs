using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using Tradera.Models;
using Tradera.Models.ServiceResolvers;
using Tradera.Services;


namespace ServicesTests
{
    public class DataProcessorTests
    {
        private Mock<INotificationsService> _notifications = new();

        [SetUp]
        public void Setup()
        {
            _notifications = new Mock<INotificationsService>();
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
        }

        [Test]
        public async Task AssertNotificationsAreSentOnFirst()
        {
            var dataProcessor = new DataProcessor(_notifications.Object);
            await dataProcessor.AddEntry(new ExchangeTicker
            {
                Identifier = new ProcessorIdentifier(ExchangeName.Binance, "btcusdt")
            });
            _notifications.Verify(c => c.DataUpdated(It.IsAny<IEnumerable<ExchangeTicker>>()), Times.Once);
        }
        
        [Test]
        public async Task AssertNotificationIsSent()
        {
            var dataProcessor = new DataProcessor(_notifications.Object);
            for (int i = 0; i < 101; i++)
            {
                await dataProcessor.AddEntry(new ExchangeTicker
                    {
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "btcusdt")
                    });
            }

            _notifications.Verify(c => c.DataUpdated(It.IsAny<IEnumerable<ExchangeTicker>>()), Times.Exactly(101)); 
        }
        
        [Test]
        public async Task AssertNotificationIsNotSent_WhenOldEventsAreReceived()
        {
            var dataProcessor = new DataProcessor(_notifications.Object);
            for (int i = 0; i < 101; i++)
            {
                await dataProcessor.AddEntry(new ExchangeTicker
                {
                    EventTime = i,
                    Identifier = new ProcessorIdentifier(ExchangeName.Binance, "btcusdt")
                });
            }
            
            await dataProcessor.AddEntry(new ExchangeTicker
            {
                EventTime = 5,
                Identifier = new ProcessorIdentifier(ExchangeName.Binance, "btcusdt")
            });
            await dataProcessor.AddEntry(new ExchangeTicker
            {
                EventTime = 5,
                Identifier = new ProcessorIdentifier(ExchangeName.Binance, "btcusdt")
            });
            
            
            _notifications.Verify(c => c.DataUpdated(It.IsAny<IEnumerable<ExchangeTicker>>()), Times.Exactly(101));
        }

    }
}