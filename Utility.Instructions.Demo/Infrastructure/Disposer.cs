using System;
using System.Collections.Generic;

namespace Utility.Instructions.Demo
{
    public sealed class Disposer<T> : IDisposable
    {
        private readonly System.Collections.IList observers;
        private readonly IObserver<T> observer;

        public Disposer(System.Collections.IList observers, IObserver<T> observer)
        {
            (this.observers = observers).Add(observer);
            this.observer = observer;
        }
        public System.Collections.IEnumerable Observers => observers;
        public IObserver<T> Observer => observer;

        public void Dispose()
        {
            observers?.Remove(observer);
        }
    }  
    
    public sealed class Disposer<T, TKey> : IDisposable
    {
        private readonly IDictionary<TKey, IObserver<T>> observers;
        private readonly TKey key;
        private readonly IObserver<T> observer;

        public Disposer(IDictionary<TKey, IObserver<T>> observers, TKey key, IObserver<T> observer)
        {
            (this.observers = observers).Add(key, observer);
            this.key = key;
            this.observer = observer;
        }

        public System.Collections.IEnumerable Observers => observers;
        public IObserver<T> Observer => observer;

        public void Dispose()
        {
            observers?.Remove(key);
        }
    }
}
