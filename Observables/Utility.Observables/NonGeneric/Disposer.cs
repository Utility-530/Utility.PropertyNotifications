using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Observables.NonGeneric
{
    public class Disposer : IDisposable
    {
        private readonly ICollection<IObserver> observers;
        private readonly IObserver observer;

        public Disposer(ICollection<IObserver> observers, IObserver observer) : this(observer)
        {
            //(this.observers = observers.ToDictionary(a => (IEquatable)a, a => a)).Add(observer, observer);
            observers.Add(observer);
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

        public static Disposer Empty => new (new Collection<IObserver>(), new Observer());

    }
    //public class Disposer<T> : IDisposable where T : IEquatable
    //{
    //    private readonly ICollection<IObserver> observers;
    //    private readonly IObserver observer;

    //    public Disposer(ICollection<IObserver> observers, IObserver observer) : this(observer)
    //    {
    //        (this.observers = observers).Add(observer);
    //    }

    //    private Disposer(IObserver observer)
    //    {
    //        this.observer = observer;
    //    }

    //    public System.Collections.IEnumerable Observers => observers;

    //    public IObserver Observer => observer;

    //    public void Dispose()
    //    {
    //        observers?.Remove(observer);
    //    }
    //}

}
