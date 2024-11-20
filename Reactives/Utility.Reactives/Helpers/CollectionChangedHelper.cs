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

namespace Utility.Reactives
{
    public static class CollectionChangedHelper
    {
        [Obsolete]
        public static IObservable<T> ToNewItemsObservable<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .Changes()
              .SelectMany(x => x.NewItems?.Cast<T>() ?? new T[] { });

        }
              
        public static IObservable<T> Additions<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .Changes()
              .SelectMany(x => x.NewItems?.Cast<T>() ?? new T[] { });

        }

        public static IObservable<T> ToOldItemsObservable<T>(this INotifyCollectionChanged notifyCollectionChanged)
        {
            return notifyCollectionChanged
              .Changes()
              .SelectMany(x => x.OldItems?.Cast<T>() ?? new T[] { });
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
                        NotifyCollectionChangedAction.Replace => throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),
                        //return new ChangeSet<T>(a.NewItems.Cast<T>().Select(c => new Change<T>(c, ChangeType.Add)).ToArray());
                        NotifyCollectionChangedAction.Move => throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),
                        //return new ChangeSet<T>(a.NewItems.Cast<T>().Select(c => new Change<T>(c, ChangeType.Add)).ToArray());
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
    }
}