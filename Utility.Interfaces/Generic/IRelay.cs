using System;
using System.Collections.Generic;

namespace Utility.Interfaces.Generic
{
    public interface IRelay<T> : IObservable<T>, IObserver<T>, IEnumerable<T>
    {
        IEnumerable<IObserver<T>> Observers { get; }
    }
}
