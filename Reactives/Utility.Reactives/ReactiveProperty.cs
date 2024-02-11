using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Utility.Reactives
{
    public class ReactiveProperty<T> : IObservable<T>
    {
        private readonly IEqualityComparer<T> equalityComparer;
        private readonly ReplaySubject<T> subject = new ReplaySubject<T>(1);
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

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subject.Subscribe(observer);
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