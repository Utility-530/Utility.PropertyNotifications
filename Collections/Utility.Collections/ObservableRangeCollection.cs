using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utility.Interfaces.Generic;

namespace Utility.Collections
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/3300845/observablecollection-calling-oncollectionchanged-with-multiple-new-items"></a>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableRangeCollection<T> : ObservableCollection<T>, IAddRange<T>
    {
        private bool suppressNotification;

        public ObservableRangeCollection()
        { }

        public ObservableRangeCollection(IEnumerable<T> items) : base(items)
        {
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChangedMultiItem(
            NotifyCollectionChangedEventArgs e)
        {
            var handlers = CollectionChanged;
            if (handlers == null) return;

            foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
            {
                var collectionViewMethod = handler.Target?.GetType().GetMethods().SingleOrDefault(a=>a.Name == "Refresh");
                if (collectionViewMethod != null)
                {
                    collectionViewMethod.Invoke(handler.Target, null);
                }
                else
                {
                    handler(this, e);
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (suppressNotification) return;

            base.OnCollectionChanged(e);
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, e);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) return;

            suppressNotification = true;

            var itemList = items.ToList();

            foreach (var item in itemList)
            {
                Add(item);
            }
            suppressNotification = false;

            if (itemList.Any())
            {
                OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemList));
            }
        }

        public void ReplaceWithRange(IEnumerable<T> items)
        {
            Items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            AddRange(items);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            suppressNotification = true;

            var removableItems = items.Where(x => Items.Contains(x)).ToList();

            foreach (var item in removableItems)
            {
                Remove(item);
            }

            suppressNotification = false;

            if (removableItems.Any())
            {
                OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removableItems));
            }
        }
    }
}