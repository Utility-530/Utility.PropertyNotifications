using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Utility.Collections
{


    /// <summary>
    /// <a href="https://gist.github.com/weitzhandler/"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortableObservableCollection<T> : RangeObservableCollection<T>
    {
        Dictionary<int, VisibleItem> filteredItems = new();

        public SortableObservableCollection()
        {
        }

        public SortableObservableCollection(IComparer<T> comparer, bool descending = false) : base()
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            IsDescending = descending;
        }

        public SortableObservableCollection(IEnumerable<T> collection, IComparer<T>? comparer, bool descending = false) : base(collection)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            IsDescending = descending;

            Sort();
        }

        private IComparer<T>? _Comparer;

        public IComparer<T> Comparer
        {
            get => _Comparer ??= Comparer<T>.Default;
            set
            {
                _Comparer = value;
                Sort();
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Comparer)));
            }
        }

        private Predicate<T> _filter;
        public Predicate<T> Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                Refresh();
            }
        }

        public void Refresh()
        {
            int index = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                bool visible = (Filter != null) ? Filter(Items[i]) : true;

                if (visible)
                {
                    filteredItems[i].VisibleIndex = index;

                    if (!filteredItems[i].Visible)
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                            filteredItems[i].Item, index));
                    }

                    index++;
                }
                else
                {
                    filteredItems[i].VisibleIndex = -1;

                    if (filteredItems[i].Visible)
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                        filteredItems[i].Item, index));
                    }
                }

                filteredItems[i].Visible = visible;
            }
        }

        [DebuggerDisplay("Index: {VisibleIndex} ; Item: {Item} ; Visible: {Visible}")]
        private class VisibleItem
        {
            public int VisibleIndex;
            public bool Visible;
            public T Item;

            public VisibleItem(int index, T item, bool visible = true)
            {
                VisibleIndex = index;
                Item = item;
                Visible = visible;
            }
        }

        private bool _IsDescending;

        /// <summary>
        /// Gets or sets a value indicating whether the sorting should be descending.
        /// Default value is false.
        /// </summary>
        public bool IsDescending
        {
            get => _IsDescending;
            set
            {
                if (_IsDescending != value)
                {
                    _IsDescending = value;
                    Sort();
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsDescending)));
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (_Reordering) return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Reset:
                    return;
            }

            Sort();
        }

        private bool _Reordering;

        public void Sort() 
        {
            _Reordering = true;
            Sort<T>(this, Comparer);
            _Reordering = false;

        }

        public static void Sort<T>(ObservableCollection<T> collection, IComparer<T> comparison)
        {

            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                if (collection.IndexOf(sortableList[i]) != i)
                    collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
    }
}