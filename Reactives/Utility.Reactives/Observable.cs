using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;

namespace Utility.Reactives
{
    public class Observable<T> : IObservable<T>, IGetReference
    {
        public readonly IObservable<T>[] observables;
        T previousValue;

        public Observable(IObservable<T>[] observables)
        {
            this.observables = observables;
            foreach (var observable in observables)
            {
                if (observable is IGetReference getReference)
                {
                    if (Reference == null)
                        Reference = getReference.Reference;
                    else if (Reference != getReference.Reference)
                    {
                        throw new ArgumentException("All observables must provide the same reference.");
                    }

                }
                else
                {
                    throw new ArgumentException("All observables must implement IGetReference to provide a reference.");
                }
            }

            this.observables = observables;
        }

        public object Reference { get; }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            CompositeDisposable compositeDisposable = new();
            foreach (var observable in observables)
            {
                observable.Subscribe(a =>
                {
                    if (a is T value)
                    {
                        if (EqualityComparer<object>.Default.Equals(value, this.previousValue)) return; // Avoid duplicates
                        observer.OnNext(value);
                    }
                }, observer.OnError, observer.OnCompleted);
            }
            ;

            return compositeDisposable;
        }
    }

}
