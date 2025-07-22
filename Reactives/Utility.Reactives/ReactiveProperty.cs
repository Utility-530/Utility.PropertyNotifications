using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Reactives
{
    public class ReactiveProperty : IReactiveProperty, ISetValue
    {
        private readonly IEqualityComparer<object> equalityComparer;
        private readonly ReplaySubject<object> subject = new(1);
        private readonly Action<object> onNext;
        private readonly Action<Exception> onException;
        private readonly Action onCompleted;
        private object value = default!;

        public ReactiveProperty()
        {
        }

        public ReactiveProperty(Action<object> onNext, Action<Exception> onException, Action onCompleted, IEqualityComparer<object> equalityComparer = default) : this(equalityComparer)
        {
            Value = value;
            this.onNext = onNext;
            this.onException = onException;
            this.onCompleted = onCompleted;
        }


        public ReactiveProperty(object value, IEqualityComparer<object> equalityComparer = default) : this(equalityComparer)
        {
            Value = value;
        }

        public ReactiveProperty(IEqualityComparer<object> equalityComparer = default) => this.equalityComparer = equalityComparer;

        public object Value
        {
            get => value;
            set
            {
                if (equalityComparer?.Equals(value, this.value) is { } b && b) return;
                this.value = value;
                subject.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return subject.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(object value)
        {
            Value = value;
            onNext?.Invoke(value);
        }
    }

    public class ReactiveProperty<T> : IObservable<T>, IObserver<T>, IValue<T>, ISetValue<T>
    {
        private readonly IEqualityComparer<T> equalityComparer;
        private readonly ReplaySubject<T> subject = new(1);
        private T value = default!;

        public ReactiveProperty()
        {
        }

        public ReactiveProperty(T value, IEqualityComparer<T> equalityComparer = default) : this(equalityComparer)
        {
            Value = value;
        }

        public ReactiveProperty(IEqualityComparer<T> equalityComparer = default) => this.equalityComparer = equalityComparer;

        public T Value
        {
            get => value;
            set
            {
                if (equalityComparer?.Equals(value, this.value) is { } b && b) return;
                this.value = value;
                subject.OnNext(value);
            }
        }

        object IValue.Value => this.value;
        object ISetValue.Value { set => Value = (T)value; }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            Value = value;
        }
    }

    public class ReactiveEquatableProperty<T> : IObservable<T> where T : IEquatable<T>
    {
        private readonly ReplaySubject<T> subject = new ReplaySubject<T>();
        private T value = default!;

        public T Value
        {
            get => value;
            set
            {
                if (value.Equals(this.value)) return;
                this.value = value;
                subject.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
        }
    }

}