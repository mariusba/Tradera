using System;

namespace Tradera.Models
{
    public interface IExchangeAgent
    {
        public IObservable<T> GetObservable<T>(ProcessorIdentifier identifier);
        public void AddObservable<T>(IObservable<T> observable, ProcessorIdentifier identifier);

    }
}