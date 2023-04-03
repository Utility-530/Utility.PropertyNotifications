using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility.Observables
{
    public interface IObservable : IEnumerable
    {

        public List<IObserver> Observers { get; }

        public IDisposable Subscribe(IObserver value)
        {
            return new Disposer(Observers, value);
        }

    }
}
