using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.NonGeneric;

namespace Utility.Infrastructure
{
    public abstract class BaseObject : IObservable
    {
        public abstract Key Key { get; }

        public IEnumerable<IObserver> Observers => Resolver.Observers(Key);

        public static IResolver? Resolver { get; set; }
        public static SynchronizationContext? Context { get; set; }

        public IDisposable Subscribe(IObserver observer)
        {
            return new Disposer(Resolver.Observers(Key), observer);
        }


        public bool Equals(IEquatable? other)
        {
            return other.Equals(this.Key);
        }

        public abstract IEnumerator GetEnumerator();

        protected void Broadcast(object obj)
        {
            foreach (var observer in Observers)
                observer.OnNext(obj);
        }

    }
}
