using System;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Reactive.NonGeneric
{
    public interface IObserver : IEquatable
    {
        void OnNext(object value);

        void OnProgress(int complete, int total);

        void OnCompleted();

        void OnError(Exception error);
    }
}