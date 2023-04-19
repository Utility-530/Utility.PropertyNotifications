using System.Collections;
using System.Reactive.Linq;
using Utility.Collections;
using Utility.Infrastructure.Abstractions;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{
    public class History : IHistory
    {
        public List<IObserver<object>> observers = new();

        private ThreadSafeObservableCollection<Order> past = new();
        private ThreadSafeObservableCollection<Order> present = new();
        private ThreadSafeObservableCollection<Order> future = new();

        public History()
        {
            ThreadSafeObservableCollection<Order>.Context = SynchronizationContext.Current;
        }

        public IEnumerable Past => past;

        public IEnumerable Present => present;

        public IEnumerable Future => future;

        public void OnNext(object order)
        {
            if (isDirty)
            {
                future.Clear();
            }
            if (order is not Order o)
            {
                throw new Exception("rfe w3");
            }

            if (future.Any(a => a.Key == o.Key && a.Access == o.Access && a.Value == o.Value))
            {
                return;
            }

            future.Add(o);
            if (present.Any())
                Broadcast(present[0]);
        }

        public void Forward()
        {
            var d = future[0];
            if (present.Count > 0)
            {
                past.Add(present[0]);
            }
            if (present.Count > 0)
                present.RemoveAt(0);
            present.Add(d);
            future.RemoveAt(0);
            Broadcast(present[0]);
        }

        private bool isDirty;

        public void Back()
        {
            isDirty = true;
            var d = past[^1];
            //if (past.Any())
            future.Insert(0, present[0]);
            if (present.Count > 0)
                present.RemoveAt(0);
            present.Add(d);
            past.Remove(d);
            Broadcast(present[0]);
        }

        private void Broadcast(object obj)
        { foreach (var observer in observers) observer.OnNext(obj); }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return new Disposer<object>(observers, observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}