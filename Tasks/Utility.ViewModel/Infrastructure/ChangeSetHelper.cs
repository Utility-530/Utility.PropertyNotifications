using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using System.Reactive.Concurrency;

namespace Utility.ViewModel
{
    public static class ChangeSetHelper
    {
        public static IObservable<IChangeSet<TOut, TValue>>
           FilterAndSelect<T, TKey, TValue, TOut>(
           this IObservable<IChangeSet<T, TKey>> observable,
           Func<T, bool> predicate,
           Func<TOut, TValue> keySelector,
           Func<T?, IObservableCache<TOut, TValue>> selector)
        {
            var collection = observable
            .Filter(a => predicate(a))
            .ToCollection()
            .Where(a => a != null)
            .Select(a => selector(a.FirstOrDefault()) ?? new SourceCache<TOut, TValue>(keySelector))
            .SelectMany(a => a.Connect());

            return collection;
        }

        public static IObservable<IChangeSet<KeyCache<T, TGroupKey, TKey>, TGroupKey>>
            SelectGroups<T, TGroupKey, TKey>(IObservable<T> observable, IScheduler scheduler)
            where T : UtilityInterface.Generic.IKey<TKey>, IGroupKey<TGroupKey>
        {
            var transforms = observable
                          .ObserveOn(scheduler)
                    .SubscribeOn(scheduler)
                        .ToObservableChangeSet(a => a.Key)
                                .Group(a => a.GroupKey)
                                .Transform(a => new KeyCache<T, TGroupKey, TKey>(a.Key, a.Cache));

            return transforms;
        }



        public static IObservable<IChangeSet<KeyList<T, TGroupKey>>>
            SelectGroupKeyGroups<T, TGroupKey, TKey>(IObservable<T> observable, IScheduler scheduler)
            where T : UtilityInterface.Generic.IKey<TKey>, IGroupKey<TGroupKey>
        {
            var transforms = observable
                          .ObserveOn(scheduler)
                          .SubscribeOn(scheduler)
                          .ToObservableChangeSet()
                          .GroupOn(a => a.GroupKey)
                          .Transform(a => new KeyList<T, TGroupKey>(a.GroupKey, a.List));

            return transforms;
        }

        public static IObservable<IChangeSet<KeyCollection>> SelectKeyGroups<T, TOut>(IObservable<T> states, IScheduler scheduler, Func<T, TOut> transform)
              where T : UtilityInterface.Generic.IKey<string>
        {
            var keyGroups = states
                .ObserveOn(scheduler)
                .SubscribeOn(scheduler)
                .ToObservableChangeSet()
                .GroupOn(a => a.Key)
                .Transform(a =>
                {
                    a.List.Connect().Transform(transform).Bind(out var gitems).Subscribe();
                    return new KeyCollection(a.GroupKey, gitems);
                });

            return keyGroups;
        }

        public static IObservable<IChangeSet<KeyList<T, TKey>>> SelectGroups<T, TKey>(IObservable<T> observable, IScheduler scheduler) where T : UtilityInterface.Generic.IKey<TKey>
        {
            var transforms = observable
                .ObserveOn(scheduler)
                .SubscribeOn(scheduler)
                .ToObservableChangeSet()
                .GroupOn(a => a.Key)
                .Transform(a => new KeyList<T, TKey>(a.GroupKey, a.List));

            return transforms;
        }
        public static IObservable<IChangeSet<KeyCollection, TGroupKey>> SelectGroupKeyGroups<T, TGroupKey, TOut>(
            IObservable<T> states, IScheduler scheduler, Func<T, TOut> func)
            where T : UtilityInterface.Generic.IKey<string>, IGroupKey<TGroupKey>
        {
            var keyGroups = states
                .ObserveOn(scheduler)
                .SubscribeOn(scheduler)
                .ToObservableChangeSet(a => a.Key)
                .Group(a => a.GroupKey)
                .Transform(a =>
                {
                    a.Cache
                    .Connect()
                    .Transform(func)
                    .Bind(out var gitems).Subscribe();

                    return new KeyCollection(a.Key.ToString(), gitems);
                });

            return keyGroups;
        }



        //public static IObservable<IChangeSet<TOut,TKey>> FilterAndSelect<T,TKey, TOut>(
        //    this IObservable<IChangeSet<T>> observable,
        //    Func<T, bool> predicate,
        //    Func<TOut, TKey> keySelector,
        //    Func<T?, IEnumerable<TOut>?> selector)
        //{
        //    var collection = observable
        //                           .Filter(a => predicate(a))
        //                           .ToCollection()
        //                           .WhereNotNull()
        //                           .SelectMany(a => selector(a.FirstOrDefault()) ?? Array.Empty<TOut>())
        //                           .ToObservableChangeSet(keySelector);

        //    return collection;
        //}

        //public static IObservable<IChangeSet<KeyCache<T, TGroupKey, TKey>, TGroupKey>>
        //SelectGroups<T, TGroupKey, TKey>(IObservable<T> observable, IScheduler scheduler)
        //where T : UtilityInterface.Generic.IKey<TKey>, IGroupKey<TGroupKey>
        //{
        //    var transforms = observable
        //                  .ObserveOn(scheduler)
        //                  .SubscribeOn(scheduler)
        //                  .ToObservableChangeSet()
        //                  .GroupOn(a => a.GroupKey)
        //                  .Transform(a => new KeyCache<T,TGroupKey,TKey>(a.GroupKey, a.List));

        //    return transforms;
        //}
       
    }
}
