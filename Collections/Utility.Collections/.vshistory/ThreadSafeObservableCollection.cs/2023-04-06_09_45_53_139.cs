using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ThreadSafeObservableCollection<T> : ObservableCollection<T>
    {
        public static SynchronizationContext Context { get; set; }

        public ThreadSafeObservableCollection(IEnumerable<T> collection) : base(collection) { }

        public ThreadSafeObservableCollection(List<T> list) : base(list) { }

        public ThreadSafeObservableCollection() : base() { }


        protected override void ClearItems()
        {
            Context.Send(new SendOrPostCallback((param) => base.ClearItems()), null);
        }

        protected override void InsertItem(int index, T item)
        {
            Context.Send(new SendOrPostCallback((param) => base.InsertItem(index, item)), null);
        }

        protected override void RemoveItem(int index)
        {
            Context.Send(new SendOrPostCallback((param) => base.RemoveItem(index)), null);
        }

        protected override void SetItem(int index, T item)
        {
            Context.Send(new SendOrPostCallback((param) => base.SetItem(index, item)), null);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            Context.Send(new SendOrPostCallback((param) => base.MoveItem(oldIndex, newIndex)), null);
        }
    }
}
