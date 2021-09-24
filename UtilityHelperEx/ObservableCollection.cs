using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace UtilityHelperEx
{
    public static class ObservableCollectionHelper
    {

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return enumerable is ObservableCollection<T> collection ? collection : new ObservableCollection<T>(enumerable);
        }

        public static IObservable<NotifyCollectionChangedEventArgs> SelectChanges(this INotifyCollectionChanged oc)
        {
            return Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => oc.CollectionChanged += h,
                h => oc.CollectionChanged -= h)
                .Select(a => a.EventArgs);
        }

        public static IObservable<T> MakeObservable<T>(this IEnumerable oc)
        {
            try
            {
                return oc is INotifyCollectionChanged notifyCollectionChanged ?
                        oc.Cast<T>().ToObservable()
                            .Concat(notifyCollectionChanged.SelectNewItems<T>()) :
                        oc.Cast<T>().ToObservable();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IObservable<T> MakeObservable<T>(this IEnumerable<T> oc)
        {
            return oc is INotifyCollectionChanged notifyCollectionChanged ?
                        oc.ToObservable()
                            .Concat(notifyCollectionChanged.SelectNewItems<T>()) :
                        oc.ToObservable();
        }

        public static IObservable<object> MakeObservable(this IEnumerable oc)
        {
            return oc.MakeObservable<object>();
        }

        public static IObservable<T> SelectNewItems<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .SelectMany(x => x.NewItems?.Cast<T>() ?? new T[] { });
        }

        public static IObservable<T> SelectOldItems<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .SelectChanges()
              .SelectMany(x => x.OldItems?.Cast<T>() ?? new T[] { });
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

        public static ObservableCollection<T> ToObservableCollection<T>(this IObservable<T> observable, IScheduler scheduler)
        {
            var obs = new ObservableCollection<T>();

            observable
                .Subscribe(a => scheduler.Schedule(() => obs.Add(a)));

            return obs;
        }

    }
}