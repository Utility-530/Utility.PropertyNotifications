using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace UtilityReactive
{
    public static class CollectionChangedHelper
    {


        public static IObservable<T> ToNewItemsObservable<T>(this INotifyCollectionChanged notifyCollectionChanged)
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

        public static IObservable<ChangeSet<T>> Changes<T>(this INotifyCollectionChanged collection) where T: IEquatable
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
                        NotifyCollectionChangedAction.Add => new ChangeSet<T>(a.NewItems.Cast<T>().Select(c => new Change<T>(c, ChangeType.Add)).ToArray()),
                        NotifyCollectionChangedAction.Remove => new ChangeSet<T>(a.OldItems.Cast<T>().Select(c => new Change<T>(c, ChangeType.Remove)).ToArray()),
                        NotifyCollectionChangedAction.Replace => throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),
                        //return new ChangeSet<T>(a.NewItems.Cast<T>().Select(c => new Change<T>(c, ChangeType.Add)).ToArray());
                        NotifyCollectionChangedAction.Move => throw new ArgumentOutOfRangeException($"{a.Serialize()} f dfde33330"),
                        //return new ChangeSet<T>(a.NewItems.Cast<T>().Select(c => new Change<T>(c, ChangeType.Add)).ToArray());
                        NotifyCollectionChangedAction.Reset => new ChangeSet<T>(new Change<T>(default, ChangeType.Reset)),
                        _ => throw new ArgumentOutOfRangeException($"{a.Serialize()} f222 dfde33330"),
                    };
                });
        }

        public static IObservable<ChangeSet<T>> Changes<T>(this IEnumerable collection) where T : IEquatable
        {
            return Observable.Create<ChangeSet<T>>(observer =>
            {
                observer.OnNext(new ChangeSet<T>(collection.Cast<T>().Select(c => new Change<T>(c, ChangeType.Add)).ToArray()));

                if (collection is INotifyCollectionChanged notifyCollection)
                    return Changes<T>(notifyCollection).Subscribe(observer);
                return Disposable.Empty;
            });
        }
    }
}