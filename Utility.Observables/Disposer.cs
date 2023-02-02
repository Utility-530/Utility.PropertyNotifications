using System;

namespace Utility.Observables
{


    public class Disposer : IDisposable
    {
        private readonly System.Collections.IList observers;
        private readonly IObserver observer;

        public Disposer(System.Collections.IList observers, IObserver observer)
        {
            (this.observers = observers).Add(observer);
            this.observer = observer;
        }

        public System.Collections.IEnumerable Observers => observers;
        public IObserver Observer => observer;

        public void Dispose()
        {
            observers?.Remove(observer);
        }
    }


    public sealed class Disposer<T> : IDisposable
    {
        private readonly System.Collections.IList observers;
        private readonly IObserver<T> observer;

        public Disposer(System.Collections.IList observers, IObserver<T> observer)
        {
            (this.observers = observers).Add(observer);
            this.observer = observer;
        }
        public Disposer(IObserver<T> observer, System.Collections.IList observers = null)
        {
            (this.observers = observers)?.Add(observer);
            this.observer = observer;
        }

        public System.Collections.IEnumerable Observers => observers;
        public IObserver<T> Observer => observer;

        public void Dispose()
        {
            observers?.Remove(observer);
        }
    }
}
