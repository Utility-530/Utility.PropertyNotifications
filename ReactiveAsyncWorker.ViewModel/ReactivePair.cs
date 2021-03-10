using ReactiveUI;
using System.Collections.Generic;

namespace ReactiveAsyncWorker.ViewModel
{
    /// <summary>
    /// From <a href="https://www.broculos.net/2014/03/wpf-editable-datagrid-and.html"></a>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReactivePair<TKey, TValue> : ReactiveObject
    {
        protected TKey key;
        protected TValue value;

        public ReactivePair()
        {
        }

        public ReactivePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public ReactivePair(KeyValuePair<TKey, TValue> kv)
        {
            Key = kv.Key;
            Value = kv.Value;
        }

        public TKey Key
        {
            get => key;
            set => this.RaiseAndSetIfChanged(ref key, value);
        }

        public TValue Value
        {
            get => value;
            set => this.RaiseAndSetIfChanged(ref this.value, value);
        }

        public static ReactivePair<TKey, TValue> Create(TKey key, TValue value)
        {
            return new ReactivePair<TKey, TValue>(key, value);
        }
    }
}