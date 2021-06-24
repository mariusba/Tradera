using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tradera.Binance;
using Tradera.Models.WebSockets;


namespace BinanceTests
{
    public class Tests
    {
        dd

        [SetUp]
        public void Setup()
        {
            
        }

        public async Task Test1()
        {


            Assert.Pass();
        }
        
        
        
        public void OnNext(IObservable<string> value)
        {
            value
                .Subscribe(System.Console.WriteLine);
        }
    }
}