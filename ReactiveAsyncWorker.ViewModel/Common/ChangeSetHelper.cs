using UtilityInterface.Generic;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using DynamicData;
using System.Reactive.Concurrency;
using ReactiveAsyncWorker.Interface;
using System.Collections.ObjectModel;

namespace ReactiveAsyncWorker.ViewModel
{
    public static class ChangeSetHelper
    {
        public static IObservable<IChangeSet<TOut, TValue>>
           FilterAndSelect<T, TKey, TValue, TOut>(
           this IObservable<IChangeSet<T, TKey>> observable,
           Func<T, bool> predicate,
           Func<TOut, TValue> keySelector,
           Func<T?, IObservableCache<TOut, TValue>?> selector)
        {

            // var cache = new SourceList<TValue>();

            var collection = observable
            .Filter(a => predicate(a))
            .ToCollection()
            .WhereNotNull()
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


        public static IObservable<IChangeSet<KeyCache<T, TGroupKey>>>
            SelectGroups2<T, TGroupKey, TKey>(IObservable<T> observable, IScheduler scheduler)
            where T : UtilityInterface.Generic.IKey<TKey>, IGroupKey<TGroupKey>
        {
            var transforms = observable
                          .ObserveOn(scheduler)
                    .SubscribeOn(scheduler)
                        .ToObservableChangeSet()
                        .GroupOn(a => a.GroupKey)
                        .Transform(a => new KeyCache<T, TGroupKey>(a.GroupKey, a.List));

            return transforms;
        }

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


        public static ReadOnlyObservableCollection<KeyCollection> SelectKeyGroups<T, TOut>(IObservable<T> states, IScheduler scheduler, Func<T, TOut> transform, out IDisposable disposable)
              where T : UtilityInterface.Generic.IKey<string>
        {
            disposable = states
                .ObserveOn(scheduler)
                .SubscribeOn(scheduler)
                .ToObservableChangeSet()
                .GroupOn(a => a.Key)
                .Transform(a =>
                {
                    a.List.Connect().Transform(transform).Bind(out var gitems).Subscribe();
                    return new KeyCollection(a.GroupKey, gitems);
                })
                .Bind(out var keyGroups)
                .Subscribe();

            return keyGroups;
        }

        public static ReadOnlyObservableCollection<KeyCollection> SelectGroupGroups<T, TGroupKey>(IObservable<T> states, IScheduler scheduler, out IDisposable disposable)
      where T : IGroupKey<TGroupKey>
        {
            disposable = states
                .ObserveOn(scheduler)
                .SubscribeOn(scheduler)
                .ToObservableChangeSet()
                .GroupOn(a => a.GroupKey)
                .Transform(a =>
                {
                    a.List.Connect().Bind(out var gitems).Subscribe();
                    return new KeyCollection(a.GroupKey.ToString(), gitems);
                })
                .Bind(out var keyGroups)
                .Subscribe();

            return keyGroups;
        }

        public static ReadOnlyObservableCollection<KeyCollection> SelectGroupGroups2<T, TGroupKey, TOut>(
            IObservable<T> states, IScheduler scheduler,Func<T,TOut> func, out IDisposable disposable)
where T : UtilityInterface.Generic.IKey<string>, IGroupKey<TGroupKey>
        {
            disposable = states
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
                })
                .Bind(out var keyGroups)
                .Subscribe();

            return keyGroups;
        }
    }

    public class KeyCache<T, TGroupKey>
    {
        private readonly ReadOnlyObservableCollection<T> collection;

        public KeyCache(TGroupKey key, IObservableList<T> observableList)
        {
            Key = key;
            observableList.Connect().Bind(out collection).Subscribe();
        }

        public TGroupKey Key { get; }
        public ReadOnlyObservableCollection<T> Collection => collection;
    }
    public class KeyCache<T, TGroupKey, TKey>
    {
        public KeyCache(TGroupKey key, IObservableCache<T, TKey> cache)
        {
            Key = key;
            Cache = cache;
        }

        public TGroupKey Key { get; }
        public IObservableCache<T, TKey> Cache { get; }
    }
    //public class KeyCache<T, TGroupKey,TKey>
    //{
    //    public KeyCache(TGroupKey key, IObservableCache<T, (TGroupKey, TKey)> cache)
    //    {
    //        Key = key;
    //        Cache = cache;
    //    }

    //    public TGroupKey Key { get; }
    //    public IObservableCache<T, (TGroupKey, TKey)> Cache { get; }
    //}
}
