using System;
using Utility.Interfaces.NonGeneric;

namespace Utility.Observables.NonGeneric
{
    public class Disposer : IDisposable
    {
        private readonly IDictionary<IEquatable, IObserver> observers;
        private readonly IObserver observer;

        public Disposer(ICollection<IObserver> observers, IObserver observer) : this(observer)
        {
            (this.observers = observers.ToDictionary(a => (IEquatable)a, a => a)).Add(observer, observer);
        }

        private Disposer(IObserver observer)
        {
            this.observer = observer;
        }

        public System.Collections.IEnumerable Observers => observers;

        public IObserver Observer => observer;

        public void Dispose()
        {
            observers?.Remove(observer);
        }
    }
    public class Disposer<T> : IDisposable where T : IEquatable
    {
        private readonly IDictionary<T, IObserver> observers;
        private readonly IObserver observer;
        private readonly T key;

        public Disposer(IDictionary<T, IObserver> observers, T key, IObserver observer) : this(observer)
        {
            (this.observers = observers).Add(key, observer);
            this.key = key;
        }

        private Disposer(IObserver observer)
        {
            this.observer = observer;
        }

        public System.Collections.IEnumerable Observers => observers;

        public IObserver Observer => observer;
        public T Key => key;

        public void Dispose()
        {
            observers?.Remove(key);
        }
    }

}
