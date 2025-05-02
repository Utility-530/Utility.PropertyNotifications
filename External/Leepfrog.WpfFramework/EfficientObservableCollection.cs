using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;



namespace Leepfrog.WpfFramework
{
    public class EfficientObservableCollection<T> : ObservableCollection<T>
    {
        public EfficientObservableCollection()
            : base()
        {
        }

        public EfficientObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }
        public EfficientObservableCollection(List<T> list)
            : base(list)
        {
        }

        public async Task AddRangeAsync(IEnumerable<T> newItems)
        {
            //this.AddLog("LIST", "locking for AddRange");
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                //this.AddLog("LIST", "locked");
                addRange_Locked(newItems);
            }
            //this.AddLog("LIST", "raising events");
            raiseResetEvents();
            //this.AddLog("LIST", "done");
        }

        public async Task AddAsync(T newItem)
        {
            //this.AddLog("LIST", "locking for Add");
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                //this.AddLog("LIST", "locked");
                Add(newItem);
            }
            //this.AddLog("LIST", "done");
        }

        public async Task AddOrReplaceAsync(T newItem, Func<T,bool> findFunction)
        {
            //this.AddLog("LIST", "locking for Add");
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                //this.AddLog("LIST", "locked");
                var existingItem = this.FirstOrDefault(f=>findFunction(f));
                if (existingItem != null)
                {
                    Remove(existingItem);
                }
                Add(newItem);
            }
            //this.AddLog("LIST", "done");
        }

        public async Task ReplaceAllAsync(IEnumerable<T> newItems)
        {
            //this.AddLog("LIST", "locking for ReplaceAll");
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                //this.AddLog("LIST", "locked");
                // CHANGE THE COLLECTION OFFLINE...
                Items.Clear();
                addRange_Locked(newItems);
            }
            //this.AddLog("LIST", "raising events");
            raiseResetEvents();
            //this.AddLog("LIST", "done");
        }

        public async Task RemoveAsync(T itemToDelete)
        {
            //this.AddLog("LIST", "locking for Remove");
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                //this.AddLog("LIST", "locked");
                Remove(itemToDelete);
            }
            //this.AddLog("LIST", "done");
        }

        public async Task AddIfNotExistsAsync(T newItem)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                if (!Contains(newItem))
                {
                    Add(newItem);
                }
            }
        }

        public async Task<T> addOrReturnExistingAsync(Predicate<T> funcFindExisting, Func<T> createNew)
        {
            using (await Mutex.LockAsync().ConfigureAwait(false))
            {
                var found = this.FirstOrDefault(item=>funcFindExisting(item));
                if (found!=null)
                {
                    return found;
                }
                var newItem = createNew();
                Add(newItem);
                return newItem;
            }
        }

        private void addRange_Locked(IEnumerable<T> newItems)
        {
            // CHANGE THE COLLECTION OFFLINE...
            foreach (var item in newItems)
            {
                Items.Add(item);
            }
        }
        private void raiseResetEvents()
        {
            // THEN RAISE THE EVENTS
            try
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPropertyChanged(new PropertyChangedEventArgs(""));
            }
            catch (Exception ex)
            {
                this.AddLog(ex);
            }

        }

        public AsyncLock Mutex { get; } = new AsyncLock();
    }
}
