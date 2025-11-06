using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Utility.Interfaces.NonGeneric;

namespace Utility.Common.Collections
{
    /// <summary>
    /// https://github.com/sorteper/ConcurrentObservableCollection/blob/master/ConcurrentObservableCollection.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // All none UI threads calls are delegated to the UI thread
    // None UI threads are not allowed to subscribe to INotifyCollectionChanged
    // UI thread INotifyCollectionChanged event handlers are not allowed to modify the collection (reentrancy is blocked)
    // GetEnumerator methods return a snapshot
    public class ConcurrentObservableCollection2<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : class
    {
        #region ctor

        private ReentrancyMonitor m_monitor = new ReentrancyMonitor();
        private readonly IDispatcher dispatcher;
        protected List<T> m_list;

        public ConcurrentObservableCollection2(IDispatcher dispatcher)
        {
            m_list = new List<T>();
            this.dispatcher = dispatcher;
        }

        public ConcurrentObservableCollection2(IDispatcher dispatcher, List<T> list)
        {
            m_list = new List<T>(list);
            this.dispatcher = dispatcher;
        }

        public ConcurrentObservableCollection2(IDispatcher dispatcher, IEnumerable<T> list)
        {
            m_list = new List<T>(list);
            this.dispatcher = dispatcher;
        }

        protected virtual bool CheckAccess()
        {
            return dispatcher.CheckAccess();
        }

        protected virtual IDispatcher Dispatcher
        {
            get { return dispatcher; }
        }

        #endregion ctor

        #region public

        public object SyncRoot { get; }

        #endregion public

        #region IList<T>

        public T this[int index]
        {
            get
            {
                if (!CheckAccess())
                {
                    T t = default;
                    lock (SyncRoot)
                        Dispatcher.Invoke(() => { t = m_list[index]; });
                    return t;
                }
                return m_list[index];
            }
            set
            {
                if (!CheckAccess())
                {
                    lock (SyncRoot)
                        Dispatcher.Invoke(() => { this[index] = value; });
                }
                CheckReentrancy();
                SetItem(index, value);
            }
        }

        // Not delegating to UI thread...
        public int Count
        {
            get { return m_list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { Add(item); });
            }
            CheckReentrancy();
            AddItem(item);
        }

        public void Clear()
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { Clear(); });
            }
            CheckReentrancy();
            ClearItems();
        }

        public bool Contains(T item)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { Contains(item); });
            }
            return ContainsItem(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { CopyTo(array, arrayIndex); });
            }
            m_list.CopyTo(array, arrayIndex);
        }

        // Note: Snapshot!
        public IEnumerator<T> GetEnumerator()
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { GetEnumerator(); });
            }
            return m_list.ToList().GetEnumerator();
        }

        public int IndexOf(T item)
        {
            if (!CheckAccess())
            {
                int index = default;
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { index = IndexOf(item); });
                return index;
            }
            return m_list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { Insert(index, item); });
            }
            CheckReentrancy();
            InsertItem(index, item);
        }

        public bool Remove(T item)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { Remove(item); });
            }
            CheckReentrancy();
            return RemoveItem(item);
        }

        public void RemoveAt(int index)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { RemoveAt(index); });
            }
            CheckReentrancy();
            RemoveItemAt(index);
        }

        // Note: Snapshot!
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!CheckAccess())
            {
                IEnumerator enumerable = null;
                lock (SyncRoot)
                    Dispatcher.Invoke(() => { enumerable = ((IEnumerable)this).GetEnumerator(); });
                return enumerable;
            }
            return m_list.ToList().GetEnumerator();
        }

        #endregion IList<T>

        #region Protected Virtual

        protected virtual void ClearItems()
        {
            m_list.Clear();
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionReset();
        }

        protected virtual bool ContainsItem(T item)
        {
            return m_list.Contains(item);
        }

        protected virtual void AddItem(T item)
        {
            var index = m_list.Count;
            m_list.Insert(m_list.Count, item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected virtual void InsertItem(int index, T item)
        {
            m_list.Insert(index, item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected virtual void SetItem(int index, T newItem)
        {
            T oldItem = m_list[index];
            m_list[index] = newItem;
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
        }

        protected virtual bool RemoveItem(T item)
        {
            var index = m_list.IndexOf(item);
            if (index < 0)
                return false;
            RemoveItemAt(index);
            return true;
        }

        protected virtual void RemoveItemAt(int index)
        {
            T item = m_list[index];
            m_list.RemoveAt(index);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        #endregion Protected Virtual

        #region IPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion IPropertyChanged

        #region INotifyCollectionChanged

        private List<NotifyCollectionChangedEventHandler> m_collectionChanged = new List<NotifyCollectionChangedEventHandler>();

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (!CheckAccess())
                    throw new InvalidOperationException("Only the UI thread can subscribe to this event");
                m_collectionChanged.Add(value);
            }
            remove
            {
                if (!CheckAccess())
                    throw new InvalidOperationException("Only the UI thread can subscribe to this event");
                m_collectionChanged.Remove(value);
            }
        }

        // https://blog.stephencleary.com/2009/07/interpreting-notifycollectionchangedeve.html
        // https://blogs.msdn.microsoft.com/xtof/2008/02/10/making-sense-of-notifycollectionchangedeventargs/
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (m_collectionChanged.Count > 0)
            {
                using (BlockReentrancy())
                {
                    foreach (var handler in m_collectionChanged)
                    {
                        if (!CheckAccess())
                        {
                            lock (SyncRoot)
                                Dispatcher.Invoke(() => { handler(this, e); });
                        }
                        else
                            handler(this, e);
                    }
                }
            }
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        protected void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion INotifyCollectionChanged

        #region Reentrancy

        private class ReentrancyMonitor : IDisposable
        {
            private int m_lockCount;
            public bool IsLocked => m_lockCount > 0;

            public void Enter()
            {
                m_lockCount++;
            }

            public void Dispose()
            {
                m_lockCount--;
            }
        }

        private IDisposable BlockReentrancy()
        {
            m_monitor.Enter();
            return m_monitor;
        }

        // https://stackoverflow.com/questions/6247427/blockreentrancy-in-observablecollectiont
        // But really. Allowing reentrancy, when just 1 subscriber exists, is just higher order idiocy
        private void CheckReentrancy()
        {
            if (m_monitor.IsLocked && m_collectionChanged.Count > 1)
                throw new InvalidOperationException("ConcurrentObservableCollection: It's not allowed to modify the collection within the event handler");
        }

        #endregion Reentrancy
    }
}