using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Collections.ObjectModel;
using DynamicData;
using ReactiveUI;
using DynamicData.Binding;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace ReactiveAsyncWorker.ViewModel
{
    public class CollectionViewModel<T, TKey> :
        CollectionViewModel,
        IObserver<IChangeSet<T, TKey>>
        where T : IComparable<T>, IComparable
    {
        const int topCount = 10;

        private readonly ReadOnlyObservableCollection<T> itemsAll;
        private readonly ReadOnlyObservableCollection<T> itemsTop;
        private readonly ISubject<IChangeSet<T, TKey>> subject = new ReplaySubject<IChangeSet<T, TKey>>();
        private readonly ISubject<NotifyCollectionChangedEventArgs> collectionChangeSubject = new ReplaySubject<NotifyCollectionChangedEventArgs>();

        public CollectionViewModel(string name, IObservable<IChangeSet<T, TKey>> changeSetObservable) : base(name)
        {
            changeSetObservable.Subscribe(subject);

            subject
                .Sort(SortExpressionComparer<T>.Descending(t => t))
                .Bind(out itemsAll)
                .Top(new Comparer<T>(), topCount)
                .Bind(out itemsTop)
                .Subscribe();

            itemsTop
                .ObserveCollectionChanges()
                .Select(a => a.EventArgs)
                .Subscribe(a =>
                {
                    collectionChangeSubject.OnNext(a);
                });
        }

        public override ICollection CollectionAll => itemsAll;

        public override ICollection CollectionTop => itemsTop;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IChangeSet<T, TKey> value)
        {
            subject.OnNext(value);
        }

        public override IDisposable Subscribe(IObserver<NotifyCollectionChangedEventArgs> observer)
        {
            return collectionChangeSubject.Subscribe(observer);
        }

        class Comparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T? x, T? y)
            {
                return x.CompareTo(y);
            }
        }
    }


    public class CollectionViewModel<T> :
        CollectionViewModel,
        IObserver<IChangeSet<T>>
        where T : IComparable
    {
        const int topCount = 10;

        private readonly ReadOnlyObservableCollection<T> itemsAll;
        private readonly ReadOnlyObservableCollection<T> itemsTop;
        private readonly ISubject<IChangeSet<T>> subject = new ReplaySubject<IChangeSet<T>>();
        private readonly ISubject<NotifyCollectionChangedEventArgs> collectionChangeSubject = new ReplaySubject<NotifyCollectionChangedEventArgs>();

        public CollectionViewModel(string name, IObservable<IChangeSet<T>> changeSetObservable) : base(name)
        {
            changeSetObservable.Subscribe(subject);

            subject
                .Sort(SortExpressionComparer<T>.Descending(t => t))
                .Bind(out itemsAll)
                .Top(topCount)
                .Bind(out itemsTop)
                .Subscribe();

            itemsTop
                .ObserveCollectionChanges()
                .Select(a => a.EventArgs)
                .Subscribe(a =>
                {
                    collectionChangeSubject.OnNext(a);
                });
        }


        public override ReadOnlyObservableCollection<T> CollectionAll => itemsAll;

        public override ReadOnlyObservableCollection<T> CollectionTop => itemsTop;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(IChangeSet<T> value)
        {
            subject.OnNext(value);
        }

        public override IDisposable Subscribe(IObserver<NotifyCollectionChangedEventArgs> observer)
        {
            return collectionChangeSubject.Subscribe(observer);
        }
    }


    public abstract class CollectionViewModel : ReactiveObject, IObservable<NotifyCollectionChangedEventArgs>
    {
        protected CollectionViewModel(string name)
        {
            Name = name;
        }

        public static CollectionViewModel<T> Create<T>(string name, IObservable<IChangeSet<T>> changeSetObservable) 
            where T : IComparable<T>, IComparable
        {
            return new CollectionViewModel<T>(name, changeSetObservable);
        }

        public static CollectionViewModel<T, TKey> Create<T, TKey>(string name, IObservable<IChangeSet<T, TKey>> changeSetObservable) 
            where T : IComparable<T>, IComparable
        {
            return new CollectionViewModel<T,TKey>(name, changeSetObservable);
        }

        public abstract IDisposable Subscribe(IObserver<NotifyCollectionChangedEventArgs> observer);

        public abstract ICollection CollectionAll { get; }

        public abstract ICollection CollectionTop { get; }

        public string Name { get; }
    }
}
