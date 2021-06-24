using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Serilog;

namespace Tradera.Models.ExchangeWrapper
{
    public class ExchangeWrapper : IObserver<IObservable<string>>, IObservable<ExchangeTicker>
    {
        private readonly IMapper _mapper;
        private readonly Subject<ExchangeTicker> _subject =  new();
        
        public ExchangeWrapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public void OnCompleted()
        {
            Log.Information("Completed wrapper");
        }

        public void OnError(Exception error)
        {
            Log.Error("error {error}", error);
        }

        public void OnNext(IObservable<string> value)
        {
            value.Select(s => _mapper.Process(s)).Subscribe(list => Notify(list));
        }
        
        private void Notify(List<ExchangeTicker> tickers)
        {
            try
            {
                foreach (var ticker in tickers)
                {
                    _subject.OnNext(ticker);   
                }
            }
            catch (Exception ex)
            {
                Log.Error("error notifying {ex}", ex);
            }
        }

        public IDisposable Subscribe(IObserver<ExchangeTicker> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}