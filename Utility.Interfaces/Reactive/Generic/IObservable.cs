using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility.Interfaces.Reactive.Generic
{
    public interface IObservable<T> : IEnumerable
    {
        IEnumerable<IObserver<T>> Observers { get; }

        IDisposable Subscribe(IObserver<T> value);
    }
}