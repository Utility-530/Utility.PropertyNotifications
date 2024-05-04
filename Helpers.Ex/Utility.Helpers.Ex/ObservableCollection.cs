using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Utility.Helpers.Ex
{
    public static class ObservableCollectionHelper
    {
        const NotifyCollectionChangedAction all = NotifyCollectionChangedAction.Add | NotifyCollectionChangedAction.Remove | NotifyCollectionChangedAction.Replace | NotifyCollectionChangedAction.Move | NotifyCollectionChangedAction.Reset;
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return enumerable is ObservableCollection<T> collection ? collection : new ObservableCollection<T>(enumerable);
        }

        public static IObservable<NotifyCollectionChangedEventArgs> SelectChanges(this INotifyCollectionChanged collection, NotifyCollectionChangedAction action = all)
        {
            return Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => collection.CollectionChanged += h,
                h => collection.CollectionChanged -= h)
                .Select(a => a.EventArgs)
                .Where(a => action.HasFlag(a.Action));
        }


        public static IObservable<NotifyCollectionChangedEventArgs> SelectExistingItemsAndChanges(this IEnumerable collection, NotifyCollectionChangedAction action = all)
        {
            return Observable.Create<NotifyCollectionChangedEventArgs>(observer =>
            {
                foreach (var item in collection)
                    observer.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

                if (collection is INotifyCollectionChanged collectionChanged)
                    return SelectChanges(collectionChanged, action).Subscribe(observer.OnNext);
                return Disposable.Empty;
            });
        }

        public static IObservable<T> SelectNewItems<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .SelectMany(x => x.NewItems?.Cast<T>() ?? Array.Empty<T>());
        }

        public static IObservable<T> SelectNewAndExistingItems<T>(this ObservableCollection<T> collection) => SelectNewAndExistingItems<T, ObservableCollection<T>>(collection);

        public static IObservable<T> SelectNewAndExistingItems<T, TCollection>(this TCollection collection) where TCollection : IEnumerable
        {
            return Observable.Create<T>(observer =>
            {
                foreach (var item in collection)
                    observer.OnNext((T)item);

                if (collection is INotifyCollectionChanged collectionChanged)
                    return SelectNewItems<T>(collectionChanged).Subscribe(observer.OnNext);
                return Disposable.Empty;
            });
        }

        public static IObservable<T> SelectOldItems<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .SelectMany(x => x.OldItems?.Cast<T>() ?? Array.Empty<T>());
        }

        public static IObservable<NotifyCollectionChangedAction> SelectActions(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .Select(x => x.Action);
        }

        /// <summary>
        /// In order to be notified of all added items the case where a collection is reset needs to be considered.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="notifyCollectionChanged"></param>
        /// <returns></returns>
        public static IObservable<T> SelectResetItems<T, TR>(this TR notifyCollectionChanged) where TR : ICollection<T>, INotifyCollectionChanged
        {
            return notifyCollectionChanged
                .SelectChanges()
                .Where(a => a.Action == NotifyCollectionChangedAction.Reset)
                .Select(a => notifyCollectionChanged.SingleOrDefault())
                .WhereNotDefault()!;
        }

        public static IObservable<T> SelectNewAndResetItems<T, TR>(this TR observableCollection) where TR : ICollection<T>, INotifyCollectionChanged
        {
            return
            observableCollection
                .SelectResetItems<T, TR>()
                .Merge(observableCollection.SelectNewItems<T>());
        }

        public static IObservable<T> SelectNewAndResetItems<T>(this ReadOnlyObservableCollection<T> observableCollection)
        {
            return observableCollection.SelectNewAndResetItems<T, ReadOnlyObservableCollection<T>>();
        }

        public static IObservable<T> SelectNewAndResetItems<T>(this ObservableCollection<T> observableCollection)
        {
            return observableCollection.SelectNewAndResetItems<T, ObservableCollection<T>>();
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IObservable<T> observable, IScheduler? scheduler = null)
        {
            var obs = new ObservableCollection<T>();

            observable
                .Subscribe(a =>
                {
                    if (scheduler == null)
                        obs.Add(a);
                    else
                        scheduler.Schedule(() => obs.Add(a));

                });

            return obs;
        }

    }
}