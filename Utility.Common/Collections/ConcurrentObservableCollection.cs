#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using Utility.Common.Contract;

namespace Utility.Common.Collection
{
    /// <summary>
    /// https://github.com/sorteper/ConcurrentObservableCollection/blob/master/ConcurrentObservableCollection.cs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // All none UI threads calls are delegated to the UI thread
    // None UI threads are not allowed to subscribe to INotifyCollectionChanged
    // UI thread INotifyCollectionChanged event handlers are not allowed to modify the collection (reentrancy is blocked)
    // GetEnumerator methods return a snapshot
    public class ConcurrentObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged, IMixedScheduler where T : class
    {
        private ReentrancyMonitor m_monitor = new ReentrancyMonitor();
        protected List<T> list;

        protected ConcurrentObservableCollection(IScheduler? scheduler = null, SynchronizationContext? context = null)
        {
            list = new List<T>();
            if (scheduler == null)
                this.Context = context ?? SynchronizationContext.Current;
            else
                this.Scheduler = scheduler;
            SyncRoot = new object();
        }

        public ConcurrentObservableCollection(List<T> list = null, IScheduler? scheduler = null, SynchronizationContext? context = null) : this(scheduler, context)
        {
            this.list = list;
        }

        public ConcurrentObservableCollection(IEnumerable<T> enumerable = null, IScheduler? scheduler = null, SynchronizationContext? context = null) : this(scheduler, context)
        {
            list = new List<T>(enumerable);
        }

        public IScheduler Scheduler { get; }

        public SynchronizationContext Context { get; }

        protected virtual bool CheckAccess()
        {
            return true;
            //if (Context != null)
            //    return SynchronizationContext.Current == Context;
            //return Scheduler != null;
        }

        public object SyncRoot { get; }

        #region IList<T>

        public T this[int index]
        {
            get
            {
                if (!CheckAccess())
                {
                    T? t = default;
                    lock (SyncRoot)
                        (this as IMixedScheduler).ScheduleAction(() => { t = list[index]; });
                    return t;
                }
                return list[index];
            }
            set
            {
                if (!CheckAccess())
                {
                    lock (SyncRoot)
                        (this as IMixedScheduler).ScheduleAction(() => { this[index] = value; });
                }
                CheckReentrancy();
                SetItem(index, value);
            }
        }

        // Not delegating to UI thread...
        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            //if (!CheckAccess())
            //{
            lock (SyncRoot)
                (this as IMixedScheduler).ScheduleAction(() => { AddItem(item); });
            //}
            //CheckReentrancy();
            //AddItem(item);
        }

        public void Clear()
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { Clear(); });
            }
            CheckReentrancy();
            ClearItems();
        }

        public bool Contains(T item)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { Contains(item); });
            }
            return ContainsItem(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { CopyTo(array, arrayIndex); });
            }
            list.CopyTo(array, arrayIndex);
        }

        // Note: Snapshot!
        public IEnumerator<T> GetEnumerator()
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { GetEnumerator(); });
            }
            return list.ToList().GetEnumerator();
        }

        public int IndexOf(T item)
        {
            if (!CheckAccess())
            {
                int index = default;
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { index = IndexOf(item); });
                return index;
            }
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { Insert(index, item); });
            }
            CheckReentrancy();
            InsertItem(index, item);
        }

        public bool Remove(T item)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { Remove(item); });
            }
            CheckReentrancy();
            return RemoveItem(item);
        }

        public void RemoveAt(int index)
        {
            if (!CheckAccess())
            {
                lock (SyncRoot)
                    (this as IMixedScheduler).ScheduleAction(() => { RemoveAt(index); });
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
                    (this as IMixedScheduler).ScheduleAction(() => { enumerable = ((IEnumerable)this).GetEnumerator(); });
                return enumerable;
            }
            return list.ToList().GetEnumerator();
        }

        #endregion IList<T>

        #region Protected Virtual

        protected virtual void ClearItems()
        {
            list.Clear();
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionReset();
        }

        protected virtual bool ContainsItem(T item)
        {
            return list.Contains(item);
        }

        protected virtual void AddItem(T item)
        {
            var index = list.Count;
            list.Insert(list.Count, item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected virtual void InsertItem(int index, T item)
        {
            list.Insert(index, item);
            OnPropertyChanged("Count");
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected virtual void SetItem(int index, T newItem)
        {
            T oldItem = list[index];
            list[index] = newItem;
            OnPropertyChanged("Item[]");
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
        }

        protected virtual bool RemoveItem(T item)
        {
            var index = list.IndexOf(item);
            if (index < 0)
                return false;
            RemoveItemAt(index);
            return true;
        }

        protected virtual void RemoveItemAt(int index)
        {
            T item = list[index];
            list.RemoveAt(index);
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

        private readonly List<NotifyCollectionChangedEventHandler> m_collectionChanged = new ();

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
                                (this as IMixedScheduler).ScheduleAction(() => { handler(this, e); });
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