using System.Collections;
using System.Collections.ObjectModel;

namespace Utility.Observables.Generic
{
    public class Disposer<TObserver, T> : IDisposable where TObserver : IObserver<T>
    {
        private readonly ICollection<TObserver> observers;
        private readonly TObserver observer;

        public Disposer(ICollection<TObserver> observers, TObserver observer)
        {
            (this.observers = observers).Add(observer);
            this.observer = observer;
        }

        public IEnumerable Observers => observers;
        public IObserver<T> Observer => observer;

        public void Dispose()
        {
            observers?.Remove(observer);
        }

        public static Disposer<TObserver, T> Empty => new(new Collection<TObserver>(), default);
    }

    //public sealed class Disposer<T> : IDisposable
    //{
    //    private readonly ICollection< Utility.Interfaces.Generic. IObserver<T>> observers;
    //    private readonly Utility.Interfaces.Generic.IObserver<T> observer;

    //    public Disposer(ICollection<Utility.Interfaces.Generic.IObserver<T>> observers, Utility.Interfaces.Generic.IObserver<T> observer)
    //    {
    //        (this.observers = observers).Add(observer);
    //        this.observer = observer;
    //    }

    //    public IEnumerable Observers => observers;
    //    public Utility.Interfaces.Generic.IObserver<T> Observer => observer;

    //    public void Dispose()
    //    {
    //        observers?.Remove(observer);
    //    }

    //    public static Disposer<T> Empty => new(new Collection<IObserver<T>>(), new Observer<T>());

    //}
    //public sealed class Disposer<T, TKey> : IDisposable
    //{
    //    private readonly IDictionary<TKey, IObserver<T>> observers;
    //    private readonly KeyValuePair<TKey, IObserver<T>> observer;

    //    public Disposer(IDictionary<TKey, IObserver<T>> observers, KeyValuePair<TKey, IObserver<T>> observer)
    //    {
    //        (this.observers = observers).Add(observer.Key, observer.Value);
    //        this.observer = observer;
    //    }

    //    public System.Collections.IEnumerable Observers => observers;
    //    public IObserver<T> Observer => observer.Value;

    //    public void Dispose()
    //    {
    //        observers?.Remove(observer);
    //    }
    //}
}