using System;
using System.Reactive.Linq;
using DynamicData;

using System.Collections.ObjectModel;

namespace Utility
{
    public class KeyList<T, TGroupKey>
    {
        private readonly ReadOnlyObservableCollection<T> collection;

        public KeyList(TGroupKey key, IObservableList<T> observableList)
        {
            Key = key;
            ObservableList = observableList;
        }

        public TGroupKey Key { get; }

        public IObservableList<T> ObservableList { get; }

    }

    public class KeyCache<T, TKey>
    {
        public KeyCache(TKey key, IObservableCache<T, TKey> cache)
        {
            Key = key;
            Cache = cache;
        }

        public TKey Key { get; }

        public IObservableCache<T, TKey> Cache { get; }
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
