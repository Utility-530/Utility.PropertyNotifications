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


        public static IObservable<IChangeSet<KeyCache<T,TGroupKey, TKey>, TGroupKey>>
            SelectGroups<T, TGroupKey, TKey>(IObservable<T> observable, IScheduler scheduler)
            where T : UtilityInterface.Generic.IKey<TKey>, IGroupKey<TGroupKey>
        {
            var transforms = observable
                          .ObserveOn(scheduler)
                    .SubscribeOn(scheduler)
                        .ToObservableChangeSet(a => a.Key)
                                .Group(a => a.GroupKey)
                                .Transform(a => new KeyCache<T,TGroupKey, TKey>(a.Key, a.Cache));

            return transforms;
        }

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
}
