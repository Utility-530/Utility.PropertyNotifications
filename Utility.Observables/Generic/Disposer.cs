using System.Collections;

namespace Utility.Observables.Generic
{
    public sealed class Disposer<T> : IDisposable
    {
        private readonly ICollection<IObserver<T>> observers;
        private readonly IObserver<T> observer;

        //public Disposer(System.Collections.IList observers, IObserver<T> observer)
        //{
        //    (this.observers = observers).Add(observer);
        //    this.observer = observer;
        //}

        public Disposer(ICollection<IObserver<T>> observers, IObserver<T> observer)
        {
            (this.observers = observers).Add(observer);
            this.observer = observer;
        }
        //public Disposer(IObserver<T> observer, System.Collections.IList observers = null)
        //{
        //    (this.observers = observers)?.Add(observer);
        //    this.observer = observer;
        //}

        public IEnumerable Observers => observers;
        public IObserver<T> Observer => observer;

        public void Dispose()
        {
            observers?.Remove(observer);
        }
    }

    public sealed class Disposer<T, TKey> : IDisposable
    {
        private readonly IDictionary<TKey, IObserver<T>> observers;
        private readonly KeyValuePair<TKey, IObserver<T>> observer;

        public Disposer(IDictionary<TKey, IObserver<T>> observers, KeyValuePair<TKey, IObserver<T>> observer)
        {
            (this.observers = observers).Add(observer.Key, observer.Value);
            this.observer = observer;
        }

        public System.Collections.IEnumerable Observers => observers;
        public IObserver<T> Observer => observer.Value;

        public void Dispose()
        {
            observers?.Remove(observer);
        }
    }
}
