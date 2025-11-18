using System.Collections.ObjectModel;
using Splat;
using Utility.Interfaces.NonGeneric;

namespace Utility.Collections
{
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        public static SynchronizationContext Context { get; set; } = SynchronizationContext.Current;

        public ThreadSafeObservableCollection(IEnumerable<T>? collection = null) : base(collection ?? Array.Empty<T>())
        {
            if (Context == null)
                Context ??= SynchronizationContext.Current ?? Locator.Current.GetService<IContext>().UI ?? throw new Exception("1DVS sdddsd");
        }

        //public ThreadSafeObservableCollection(List<T> list) : base(list)
        //{
        //    if (Context == null)
        //        Context ??= SynchronizationContext.Current ?? Locator.Current.GetService<IContext>().UI ?? throw new Exception("2DVS sdddsd");
        //}

        //public ThreadSafeObservableCollection() : base()
        //{
        //    if (Context == null)
        //        Context ??= SynchronizationContext.Current ?? Locator.Current.GetService<IContext>().UI ?? throw new Exception("DVS sdddsd");
        //}

        #region Collection Events

        private readonly List<Action<T>> _added = new List<Action<T>>();
        private readonly List<Action<T>> _removed = new List<Action<T>>();
        private readonly List<Action<IList<T>>> _cleared = new List<Action<IList<T>>>();

        public ThreadSafeObservableCollection<T> WhenAdded(Action<T> added)
        {
            if (added != null)
            {
                _added.Add(added);
            }
            return this;
        }

        public ThreadSafeObservableCollection<T> WhenRemoved(Action<T> removed)
        {
            if (removed != null)
            {
                _removed.Add(removed);
            }
            return this;
        }

        public ThreadSafeObservableCollection<T> WhenCleared(Action<IList<T>> cleared)
        {
            if (cleared != null)
            {
                _cleared.Add(cleared);
            }
            return this;
        }

        protected virtual void NotifyOnItemAdded(T item)
        {
            for (int i = 0; i < _added.Count; i++)
            {
                _added[i](item);
            }
        }

        protected virtual void NotifyOnItemRemoved(T item)
        {
            for (int i = 0; i < _removed.Count; i++)
            {
                _removed[i](item);
            }
        }

        protected virtual void NotifyOnItemsCleared(IList<T> items)
        {
            for (int i = 0; i < _cleared.Count; i++)
            {
                _cleared[i](items);
            }
        }

        #endregion Collection Events

        protected override void ClearItems()
        {
            Context.Send(new SendOrPostCallback((param) =>
            {
                var items = new List<T>(this);
                base.ClearItems();
                if (_cleared.Count > 0)
                {
                    NotifyOnItemsCleared(items);
                }
                else
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        NotifyOnItemRemoved(items[i]);
                    }
                }
            }), null);
        }

        protected override void InsertItem(int index, T item)
        {
            Context.Send(new SendOrPostCallback((param) =>
            {
                base.InsertItem(index, item);
                NotifyOnItemAdded(item);
            }), null);
        }

        protected override void RemoveItem(int index)
        {
            Context.Send(new SendOrPostCallback((param) =>
            {
                var item = base[index];
                base.RemoveItem(index);
                NotifyOnItemRemoved(item);
            }), null);
        }

        protected override void SetItem(int index, T item)
        {
            Context.Send(new SendOrPostCallback((param) =>
            {
                T prev = base[index];
                base.SetItem(index, item);
                NotifyOnItemRemoved(prev);
                NotifyOnItemAdded(item);
            }), null);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            Context.Send(new SendOrPostCallback((param) => base.MoveItem(oldIndex, newIndex)), null);
        }
    }
}