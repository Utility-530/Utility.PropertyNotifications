using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;

namespace Utility.Helpers.Ex
{
    public static class ObservableChangeSetHelper
    {
        public static ReadOnlyObservableCollection<T> ToCollection<T, TKey>(this IObservable<IChangeSet<T, TKey>> observable, out IDisposable disposable)
            where TKey : notnull
        {
            disposable = observable
                .Bind(out var collection)
                .Subscribe();

            return collection;
        }

        public static ReadOnlyObservableCollection<T> ToCollection<T>(this IObservable<IChangeSet<T>> observable, out IDisposable disposable)
        {
            disposable = observable
                .Bind(out var collection)
                .Subscribe();

            return collection;
        }

        public static ReadOnlyObservableCollection<T> ToCollection<T, TKey>(this IObservable<IChangeSet<T, TKey>> observable, ICollection<IDisposable> disposable)
            where TKey : notnull
        {
            _ = observable
                .Bind(out var collection)
                .Subscribe()
                .DisposeWith(disposable);

            return collection;
        }

        public static ReadOnlyObservableCollection<T> ToCollection<T>(this IObservable<IChangeSet<T>> observable, ICollection<IDisposable> disposable)
        {
            _ = observable
                .Bind(out var collection)
                .Subscribe()
                .DisposeWith(disposable);

            return collection;
        }
    }
}