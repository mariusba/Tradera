using System;
using System.Collections.Generic;
using Tradera.Models;

namespace Tradera.Binance
{
    
    public class ExchangeAgent : IExchangeAgent
    {
        private readonly Dictionary<ProcessorIdentifier, object> _observables = new();

        public void AddObservable<T>(IObservable<T> observable, ProcessorIdentifier identifier)
        {
            if(!_observables.ContainsKey(identifier))
                _observables.Add(identifier, observable);
        }
        
        public IObservable<T> GetObservable<T>(ProcessorIdentifier identifier)
        {
            var ob = _observables.GetValueOrDefault(identifier) as IObservable<T>;
            return ob;
        }
    }
}