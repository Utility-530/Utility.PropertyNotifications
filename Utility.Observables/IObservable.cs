using System;
using System.Collections.Generic;

namespace Utility.Observables
{
    public interface IObservable
    {

        public List<IObserver> Observers { get; }

        public IDisposable Subscribe(IObserver value)
        {
            return new Disposer(Observers, value);
        }

    }
}
