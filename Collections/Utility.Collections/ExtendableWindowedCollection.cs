using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Utility.Collections
{
    /// <summary>
    /// Production-ready virtualizing collection for unknown-length or lazy sources.
    /// Uses placeholders and supports DataGrid virtualization safely.
    /// </summary>
    public class ExtendableWindowedCollection<T> : IList, INotifyCollectionChanged
    {
        private readonly IEnumerable<T> source;
        private readonly List<object> items;      // backing list, includes placeholders
        private readonly Dictionary<int, T> cache; // fetched real items
        private readonly object placeholder;

        private IEnumerator<T>? enumerator; // lazy enumerator
        private int lastFetchedIndex = -1;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Initialize with source enumerable, placeholder object, and initial viewport/buffer count.
        /// </summary>
        public ExtendableWindowedCollection(IEnumerable<T> source, object placeholder, int initialCount = 50)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.placeholder = placeholder ?? throw new ArgumentNullException(nameof(placeholder));
            this.items = new List<object>(initialCount);
            this.cache = new Dictionary<int, T>();

            for (int i = 0; i < initialCount; i++)
                items.Add(placeholder);

            this.logicalCount = items.Count;
            this.enumerator = source.GetEnumerator();
        }

        private int logicalCount;

        /// <summary>
        /// Get/set items by index. Returns cached item if available, otherwise placeholder.
        /// Updates WPF via Replace when a placeholder is replaced with real item.
        /// </summary>
        public object this[int index]
        {
            get
            {
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                EnsureFetched(index);

                // Return cached item if available
                if (cache.TryGetValue(index, out var real))
                {
                    if (!ReferenceEquals(items[index], real))
                    {
                        var old = items[index];
                        items[index] = real;
                        CollectionChanged?.Invoke(this,
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Replace, real, old, index));
                    }

                    return real;
                }

                // Return placeholder if not yet fetched
                return items[index];
            }
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Lazily enumerate source up to requested index.
        /// </summary>
        private void EnsureFetched(int index)
        {
            if (enumerator == null || index <= lastFetchedIndex)
                return;

            int i = lastFetchedIndex + 1;
            while (i <= index)
            {
                if (!enumerator.MoveNext())
                {
                    enumerator.Dispose();
                    enumerator = null;
                    break; // reached end
                }

                cache[i] = enumerator.Current;
                lastFetchedIndex = i;
                i++;
            }
        }

        /// <summary>
        /// Shift the visible window (viewport) and optionally extend placeholders.
        /// </summary>
        public void ShiftWindow(int firstVisibleIndex, int viewportSize, int buffer = 5)
        {
            if (enumerator == null)
                return;
            int windowEnd = firstVisibleIndex + viewportSize + buffer;

            if (windowEnd >= items.Count)
                ExtendPlaceholders(windowEnd);
        }

        /// <summary>
        /// Safely extend the collection with placeholders.
        /// Uses single-item Add for small batches, Reset for large batches.
        /// </summary>
        private void ExtendPlaceholders(int requiredIndex)
        {
            int oldCount = items.Count;
            int newCount = Math.Max(requiredIndex + 1, oldCount + oldCount / 2);
            const int ResetThreshold = 1;

            if (newCount <= oldCount)
                return;

            if (newCount - oldCount > ResetThreshold)
            {
                for (int i = oldCount; i < newCount; i++)
                    items.Add(placeholder);

                logicalCount = items.Count;
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                for (int i = oldCount; i < newCount; i++)
                {
                    items.Add(placeholder);
                    logicalCount = items.Count;

                    CollectionChanged?.Invoke(this,
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add, placeholder, i));
                }
            }
        }

        #region IList Implementation
        public int Count => logicalCount;
        public bool IsReadOnly => true;
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public IEnumerator GetEnumerator() => System.Linq.Enumerable.Range(0, Count).GetEnumerator();
        public int Add(object value) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(object value) => false;
        public int IndexOf(object value) => -1;
        public void Insert(int index, object value) { }
        public void Remove(object value) => throw new NotSupportedException();
        public void RemoveAt(int index) => throw new NotSupportedException();
        public void CopyTo(Array array, int index) { }
        #endregion
    }
}
