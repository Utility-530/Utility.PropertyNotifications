using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility.Interfaces.Generic
{
    public interface IObservable<T> : IEnumerable
    {
        IEnumerable<IObserver<T>> Observers { get; }

        IDisposable Subscribe(Utility.Interfaces.Generic.IObserver<T> value);

    }
}
