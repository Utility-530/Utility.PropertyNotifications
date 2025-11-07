using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using O = System.Reactive.Linq.Observable;
using Type = Utility.Changes.Type;

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
            return Observable.Create<T>(observer =>
            {
                return notifyCollectionChanged
                .Changes()
                .Where(a => a.Action == NotifyCollectionChangedAction.Add)
                .Subscribe(a =>
                {
                    foreach (var t in a.NewItems.Cast<T>())
                        observer.OnNext(t);
                });
            });
        }

        public static IObservable<T> Additions<T>(this IEnumerable enumerable)
        {
            if (enumerable is INotifyCollectionChanged changed)
                return changed.Additions<T>();
            else
                return Observable.Empty<T>();
        }

        public static IObservable<object> Additions(this IEnumerable enumerable)
        {
            return Additions<object>(enumerable);
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
            return collection is INotifyCollectionChanged collectionChanged ?
                collectionChanged
                .Changes()
                .Where(a => a.Action == NotifyCollectionChangedAction.Remove)
                .SelectMany(x => x.OldItems?.Cast<T>() ?? [])
                : O.Empty<T>();
        }

        public static IObservable<(T @new, T old)> Replacements<T>(this IEnumerable collection)
        {
            return collection is INotifyCollectionChanged collectionChanged ?
                collectionChanged
                .Changes()
                .Where(a => a.Action == NotifyCollectionChangedAction.Replace)
                .SelectMany(x => x.NewItems?.Cast<T>().Join(x.OldItems?.Cast<T>(), a => true, b => true, (a, b) => (a, b)))
                : O.Empty<(T @new, T old)>();
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
            return O
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
                        NotifyCollectionChangedAction.Replace => new Set<T>(create<T>(a)),  /* throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),*/
                        NotifyCollectionChangedAction.Move => throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),
                        NotifyCollectionChangedAction.Reset => new Set<T>(new Change<T>(default, Type.Reset)),
                        _ => throw new ArgumentOutOfRangeException($"{a.Serialize()} f222 dfde33330"),
                    };
                });

            static Change<T>[] create<T>(NotifyCollectionChangedEventArgs a)
            {
                var x = a.OldItems.Cast<T>().Join(a.NewItems.Cast<T>(), a => true, a => true, (oldItem, newItem) => (oldItem, newItem)).Select(c => new Change<T>(c.newItem, c.oldItem, Type.Update)).ToArray();
                return x;
            }
        }

        public static IObservable<Set<T>> AndChanges<T>(this IEnumerable collection, bool includeInitial = true)
        {
            return Observable.Create<Set<T>>(observer =>
            {
                if (collection.Any())
                    observer.OnNext(new Set<T>(collection.Cast<T>().Select(c => new Change<T>(c, Type.Add)).ToArray()));
                else if (includeInitial)
                    observer.OnNext(new Set<T>([Change<T>.None]));

                if (collection is INotifyCollectionChanged notifyCollection)
                    return Changes<T>(notifyCollection).Subscribe(observer);
                return Disposable.Empty;
            });
        }

        public static IObservable<int> Counts(this IEnumerable collection, bool includeInitial = true)
        {
            return collection
                  .AndChanges<object>(includeInitial)
                  .Select(x => collection.Count())
                  .StartWith(collection.Count())
                  .DistinctUntilChanged();
        }

        public static IObservable<T> AndAdditions<T>(this IEnumerable collection)
        {
            return Observable.Create<T>(observer =>
            {
                foreach (var x in collection.Cast<T>().ToArray())
                    observer.OnNext(x);

                if (collection is INotifyCollectionChanged notifyCollection)
                    return Additions<T>(notifyCollection).Subscribe(observer);
                return Disposable.Empty;
            });
        }

        public static IObservable<T> AndAdditions<T>(this IEnumerable<T> collection)
        {
            return Observable.Create<T>(observer =>
            {
                foreach (var x in collection.ToArray())
                    observer.OnNext(x);

                if (collection is INotifyCollectionChanged notifyCollection)
                    return Additions<T>(notifyCollection).Subscribe(observer);
                return Disposable.Empty;
            });
        }

        public static IObservable<object> AndAdditions(this IEnumerable collection) => AndAdditions<object>(collection);

        public static IObservable<TR> SelfAndAdditions<T, TR>(this T collection) where T : IEnumerable<TR>, INotifyCollectionChanged
        {
            return O.Create<TR>(observer =>
            {
                foreach (var x in collection)
                    observer.OnNext(x);

                return Additions<TR>((INotifyCollectionChanged)collection).Subscribe(observer);
            });
        }

        public static IObservable<TR> SelfAndAdditions<TR>(this IEnumerable collection)
        {
            return O.Create<TR>(observer =>
            {
                foreach (TR x in collection)
                    observer.OnNext(x);
                if (collection is INotifyCollectionChanged incc)
                    return Additions<TR>(incc).Subscribe(observer);
                return Disposable.Empty;
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
                            .Merge(collection.SelfAndAdditions<T, TR>().Select(Change<TR>.Add)))
                            .StartWith(Change<TR>.None);
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