using System;
using System.Collections;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.NonGeneric;

namespace Utility.Infrastructure
{
    public abstract class BaseObject : BaseViewModel, IObservable, IKey<Key>
    {
        private readonly List<object> list = new();

        public abstract Key Key { get; }

        public IEnumerable<IObserver> Observers => Resolver.Observers(Key);

        public static IResolver? Resolver { get; set; }
        public static SynchronizationContext? Context { get; set; }

        public IDisposable Subscribe(IObserver observer)
        {
            return new Disposer(Resolver.Observers(Key), observer);
        }
        public bool Equals(IKey<Key>? other)
        {
            return other?.Key.Equals(Key) ?? false;
        }

        public bool Equals(IEquatable? other)
        {
            return (other as IKey<Key>)?.Equals(this.Key) ?? false;
        }

        public IEnumerator GetEnumerator() => list.GetEnumerator();

        protected void Broadcast(object obj)
        {
            list.Add(obj);
            (Context ?? throw new Exception("missing context"))
                .Post(a =>
            {
                foreach (var observer in Observers)
                    observer.OnNext(obj);
            }, default);
        }
    }
}
