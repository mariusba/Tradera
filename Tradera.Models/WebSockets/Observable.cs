using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Tradera.Models.ServiceResolvers;

namespace Tradera.Models.WebSockets
{
    public class Observable : IObservable<string>
    {
        private readonly IObservable<string> _observable;

        public Observable(DataProvider dataProvider, ExchangeName provider, string pair)
        {
            _observable = System.Reactive.Linq.Observable.Using(
                async token => await dataProvider.GetDataProviderAsync(provider, pair, token),
                (webSocket, token) =>
                {
                    try
                    {
                        var buffer = WebSocket.CreateClientBuffer(dataProvider.GetChunkSize(provider),
                            dataProvider.GetChunkSize(provider));

                        var observable = System.Reactive.Linq.Observable.Create<string>(async (observer, subToken) =>
                        {
                            while (webSocket.State == WebSocketState.Open && !subToken.IsCancellationRequested)
                            {
                                var stream = new MemoryStream(1024 * 8);
                                WebSocketReceiveResult result;
                                do
                                {
                                    result = await webSocket.ReceiveAsync(buffer, subToken);
                                    stream.Write(buffer.Array, 0, result.Count);
                                } while (!result.EndOfMessage);

                                if (result.MessageType == WebSocketMessageType.Text)
                                {

                                    stream.Seek(0, SeekOrigin.Begin);
                                    var bufferRead = new ArraySegment<byte>(new byte[stream.Length]);
                                    stream.Read(bufferRead.Array, 0, (int) stream.Length);

                                    var e = bufferRead.Skip(0).Take(bufferRead.Count);
                                    var str = Encoding.UTF8.GetString(e.ToArray());
                                    observer.OnNext(str);
                                    stream.Dispose();
                                }
                            }
                        });

                        return Task.FromResult(observable);
                    }
                    catch (Exception ex)
                    {
                     Log.Error("error in observer {ex}", ex);   
                    }

                    return null;
                });
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            return _observable.Subscribe(observer);
        }

        public IDisposable Subscribe(ExchangeWrapper.ExchangeWrapper observer)
        {
            throw new NotImplementedException();
        }
    }
}