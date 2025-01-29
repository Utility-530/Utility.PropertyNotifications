using NetFabric.Hyperlinq;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Changes;
using Type = Utility.Changes.Type;
using System.Collections.Generic;
using O = System.Reactive.Linq.Observable;
using System.Collections.ObjectModel;

namespace Utility.Reactives
{
    public static class CollectionChangedHelper
    {
        [Obsolete]
        public static IObservable<T> ToNewItemsObservable<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .Changes()
              .Where(a => a.Action == NotifyCollectionChangedAction.Add)
              .SelectMany(x => x.NewItems?.Cast<T>() ?? []);

        }

        public static IObservable<T> Additions<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
                .Changes()
                .Where(a=>a.Action == NotifyCollectionChangedAction.Add)
                .SelectMany(x => x.NewItems?.Cast<T>() ?? []);

        }

        public static IObservable<T> ToOldItemsObservable<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
                .Changes()
                .Where(a => a.Action == NotifyCollectionChangedAction.Remove)
                .SelectMany(x => x.OldItems?.Cast<T>() ?? []);
        }

        public static IObservable<T> Subtractions<T>(this IEnumerable collection)
        {
            return collection is INotifyCollectionChanged collectionChanged?
                collectionChanged
                .Changes()
                .Where(a => a.Action == NotifyCollectionChangedAction.Remove)
                .SelectMany(x => x.OldItems?.Cast<T>() ?? [])
                :Observable.Empty<T>();
        }

        public static IObservable<NotifyCollectionChangedAction> ToActionsObservable<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
                .Changes()
                .Select(x => x.Action);
        }

        public static IObservable<NotifyCollectionChangedEventArgs> Changes(this INotifyCollectionChanged collection)
        {
            return Observable
                   .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                      handler => (sender, args) => handler(args),
                        handler => collection.CollectionChanged += handler,
                        handler => collection.CollectionChanged -= handler);
        }

        public static IObservable<Set<T>> Changes<T>(this INotifyCollectionChanged collection)
        {
            return Observable
                .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => (sender, args) => handler(args),
                handler => collection.CollectionChanged += handler,
                handler => collection.CollectionChanged -= handler)
                .Select(a =>
                {
                    return a.Action switch
                    {
                        NotifyCollectionChangedAction.Add => new Set<T>(a.NewItems.Cast<T>().Select(c => new Change<T>(c, Type.Add)).ToArray()),
                        NotifyCollectionChangedAction.Remove => new Set<T>(a.OldItems.Cast<T>().Select(c => new Change<T>(c, Type.Remove)).ToArray()),
                        NotifyCollectionChangedAction.Replace => new Set<T>(a.OldItems.Cast<T>().Join(a.NewItems.Cast<T>(), a => a, a => a, (oldItem, newItem) => (oldItem, newItem)).Select(c => new Change<T>(c.newItem, c.oldItem, Type.Update)).ToArray()),  /* throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),*/
                        NotifyCollectionChangedAction.Move => throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),
                        NotifyCollectionChangedAction.Reset => new Set<T>(new Change<T>(default, Type.Reset)),
                        _ => throw new ArgumentOutOfRangeException($"{a.Serialize()} f222 dfde33330"),
                    };
                });
        }

        public static IObservable<Set<T>> AndChanges<T>(this IEnumerable collection)
        {
            return Observable.Create<Set<T>>(observer =>
            {
                if (collection.Any())
                    observer.OnNext(new Set<T>(collection.Cast<T>().Select(c => new Change<T>(c, Type.Add)).ToArray()));

                if (collection is INotifyCollectionChanged notifyCollection)
                    return Changes<T>(notifyCollection).Subscribe(observer);
                return Disposable.Empty;
            });
        }


        public static IObservable<T> AndAdditions<T>(this IEnumerable collection)
        {
            return Observable.Create<T>(observer =>
            {
                if (collection.Any())
                    foreach (var x in collection.Cast<T>())
                        observer.OnNext(x);

                if (collection is INotifyCollectionChanged notifyCollection)
                    return Additions<T>(notifyCollection).Subscribe(observer);
                return Disposable.Empty;
            });
        }



        public static IObservable<TR> SelfAndAdditions<T, TR>(this T collection) where T : IEnumerable<TR>, INotifyCollectionChanged
        {
            return O.Create<TR>(observer =>
            {
                foreach (var x in collection)
                    observer.OnNext(x);

                return Additions<TR>(collection).Subscribe(observer);
            });
        }

        public static IObservable<TR> Subtractions<T, TR>(this T collection) where T : IEnumerable<TR>, INotifyCollectionChanged
        {
            return Subtractions<TR>(collection);
        }

        public static IObservable<(TR @new, TR @old)> Replacements<T, TR>(this T collection) where T : IEnumerable<TR>, INotifyCollectionChanged
        {
            return O.Create<(TR, TR)>(observer =>
            {
                return Changes(collection)
                    .Where(a => a.Action == NotifyCollectionChangedAction.Replace)
                    .Subscribe(a =>
                    {
                        var enumerator1 = a.NewItems.GetEnumerator();
                        var enumerator2 = a.OldItems.GetEnumerator();
                        while (enumerator1.MoveNext() && enumerator2.MoveNext())
                        {
                            observer.OnNext(new((TR)enumerator1.Current, (TR)enumerator2.Current));
                        }
                    });
            });
        }

        public static IObservable<Change<TR>> Changes<T, TR>(this T collection) where T : IEnumerable<TR>, INotifyCollectionChanged
        {
            return collection.Replacements<T, TR>().Select(a => Change<TR>.Update(a.@new, a.old))
                            .Merge(collection.Subtractions<T, TR>().Select(Change<TR>.Remove)
                            .Merge(collection.SelfAndAdditions<T, TR>().Select(Change<TR>.Add)));
        }



        public static IObservable<TR> SelfAndAdditions<TR>(this ObservableCollection<TR> collection)
        {
            return SelfAndAdditions<ObservableCollection<TR>, TR>(collection);
        }

        public static IObservable<TR> Subtractions<TR>(this ObservableCollection<TR> collection)
        {
            return Subtractions<ObservableCollection<TR>, TR>(collection);
        }
        public static IObservable<Change<TR>> Changes<TR>(this ObservableCollection<TR> collection)
        {
            return Changes<ObservableCollection<TR>, TR>(collection);
        }
        public static IObservable<(TR, TR)> Replacements<TR>(this ObservableCollection<TR> collection)
        {
            return Replacements<ObservableCollection<TR>, TR>(collection);
        }
    }
}