using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility.Interfaces.Reactive.NonGeneric
{
    public interface IObservable : IEnumerable
    {
        public IEnumerable<IObserver> Observers { get; }

        public IDisposable Subscribe(IObserver value);
    }
}