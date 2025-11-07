using System;
using System.Collections.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Reactives
{
    public class Observer<T> : IObserver<T>, IReference
    {
        private readonly Action<T> onNext;
        private readonly Action<Exception> onError;
        private readonly Action onCompleted;

        public Observer(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public Observer()
        {
        }

        public object Reference { get; set; }

        public List<object> Observations { get; } = new();

        public virtual void OnNext(T value)
        {
            (onNext ?? throw new NotImplementedException()).Invoke(value);
        }

        public void OnCompleted()
        {
            (onCompleted ?? throw new NotImplementedException()).Invoke();
        }

        public void OnError(Exception error)
        {
            (onError ?? throw new NotImplementedException()).Invoke(error);
        }

        public bool Equals(IEquatable other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public void OnStarted()
        {
            throw new NotImplementedException();
        }
    }
}