using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splat;
using Utility.Interfaces.NonGeneric;
using static System.Net.Mime.MediaTypeNames;

namespace Utility.Collections
{
    public interface IPageProvider<T>
    {
        Task<IReadOnlyList<T>> FetchPageAsync(
            int pageIndex,
            int pageSize,
            CancellationToken ct);
    }
    public sealed class VirtualizingCollection<T> : BaseVirtualisingCollection, IList, INotifyCollectionChanged
    {
        private readonly int count;
        private readonly Func<int, T> loader;
        private readonly Dictionary<int, T> cache = new();

        public VirtualizingCollection(int count, Func<int, T> loader) : base(count)
        {
            this.count = count;
            this.loader = loader;
        }

        public object this[int index]
        {
            get
            {
                if (!cache.TryGetValue(index, out var item))
                {
                    item = loader(index);   // DB fetch here
                    cache[index] = item;
                }
                return item!;
            }
            set => throw new NotSupportedException();
        }
    }

    public sealed class VirtualizingPaginatedCollection<T> :
        BaseVirtualisingCollection,
        IList,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private readonly IPageProvider<T> provider;
        private readonly int pageSize;
        private readonly int count;
        private readonly Dictionary<int, DataPage> pages = new();
        private readonly SemaphoreSlim gate = new(1, 1);



        public VirtualizingPaginatedCollection(
            IPageProvider<T> provider,
            int count,
            int pageSize = 100) : base(count)
        {

            this.provider = provider;
            this.count = count;
            this.pageSize = pageSize;
        }




        public object? this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                int pageIndex = index / pageSize;
                int pageOffset = index % pageSize;

                if (!pages.TryGetValue(pageIndex, out var page))
                {
                    _ = LoadPageAsync(pageIndex);
                    return default; // placeholder
                }

                page.LastAccess = DateTime.UtcNow;
                return page.Items[pageOffset];
            }
            set => throw new NotSupportedException();
        }
        internal sealed class DataPage
        {
            public int PageIndex { get; }
            public IReadOnlyList<T> Items { get; }
            public DateTime LastAccess { get; set; }

            public DataPage(int pageIndex, IReadOnlyList<T> items)
            {
                PageIndex = pageIndex;
                Items = items;
                LastAccess = DateTime.UtcNow;
            }
        }


        private async Task LoadPageAsync(int pageIndex)
        {
            await gate.WaitAsync();
            try
            {
                if (pages.ContainsKey(pageIndex))
                    return;

                var items = await provider.FetchPageAsync(
                    pageIndex,
                    pageSize,
                    CancellationToken.None);

                pages[pageIndex] = new DataPage(pageIndex, items);

                int start = pageIndex * pageSize;
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        items.ToList(),
                        Enumerable.Repeat(default(T), items.Count).ToList(),
                        start));

                EvictOldPages();
            }
            finally
            {
                gate.Release();
            }

        }

        private void EvictOldPages(int maxPages = 5)
        {
            if (pages.Count <= maxPages)
                return;

            var toRemove = pages
                .OrderBy(p => p.Value.LastAccess)
                .Take(pages.Count - maxPages)
                .Select(p => p.Key)
                .ToList();

            foreach (var key in toRemove)
                pages.Remove(key);
        }

    }

    public class BaseVirtualisingCollection:INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly int count;

        public BaseVirtualisingCollection(int count)
        {
            if (Context == null)
                Context ??= SynchronizationContext.Current ?? Locator.Current.GetService<IContext>().UI ?? throw new Exception("1DVS sdddsd");
            this.count = count;
        }
        public SynchronizationContext Context { get; }

        public int Count => count;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        public bool IsReadOnly => true;
        public bool IsFixedSize => true;
        public bool IsSynchronized => false;
        public object SyncRoot => this;
        public int Add(object value) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(object value) => false;
        public int IndexOf(object value) => -1;
        public void Insert(int index, object value) => throw new NotSupportedException();
        public void Remove(object value) => throw new NotSupportedException();
        public void RemoveAt(int index) => throw new NotSupportedException();
        public void CopyTo(Array array, int index) { }
        public IEnumerator GetEnumerator() => Enumerable.Range(0, Count).GetEnumerator();

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e) =>
            Context.Send(a => CollectionChanged?.Invoke(this, e), null);
    }
}