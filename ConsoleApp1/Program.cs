using System;
using CryptOnion.Application.Terminal;
using Spectre.Console;
using Tradera.Binance;
using Tradera.Models.WebSockets;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Tradera.Models;
using Tradera.Models.ExchangeWrapper;
using Tradera.Models.ServiceResolvers;
using Tradera.Services;
using Observable = Tradera.Models.WebSockets.Observable;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var socketconf = new BinanceWebsocketConfigurator();

            IWebSocketConfigurator[] k = new []
            {
                new BinanceWebsocketConfigurator(),
            };
            
            var provider = new DataProvider(k);
            var ob = new Observable(provider, ExchangeName.Binance, "btcusdt");
            
            var e = new BinanceAgent();
            e.AddObservable(ob);
            
            var terminal = new Terminal(AnsiConsole.Console);
            var oba = e.GetObservable<string>();
            var mapper = new BinanceMapper();
            var proc = new ExchangeWrapper(mapper);
            oba.Window(TimeSpan.FromTicks(2)).Subscribe(proc);
            var processor = new DataProcessor(new NotificationsService());
            proc.Subscribe(k => processor.AddEntry(k));
            oba.Window(TimeSpan.FromMilliseconds(3)).Subscribe(terminal); 
            await terminal.Start();
        }
    }
}