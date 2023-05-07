using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IObserver : IEquatable
    {
        //public List<object> Observations { get; }
        bool OnNext(object value);

        void OnStarted();

        void OnCompleted();

        void OnError(Exception error);
    }
}
