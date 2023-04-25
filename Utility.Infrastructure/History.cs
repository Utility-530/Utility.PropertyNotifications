using System.Collections;
using Utility.Collections;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;


namespace Utility.PropertyTrees.Infrastructure
{
    public class History : BaseObject, IHistory
    {
        private ThreadSafeObservableCollection<Order> past = new();
        private ThreadSafeObservableCollection<Order> present = new();
        private ThreadSafeObservableCollection<Order> future = new();

        public History()
        {
            ThreadSafeObservableCollection<Order>.Context = Context;
        }

        public IEnumerable Past => past;

        public IEnumerable Present => present;

        public IEnumerable Future => future;

        public override Key Key => new(default, nameof(History), typeof(History));

        public void OnNext(object order)
        {
            if (order is Direction direction)
            {
                switch (direction)
                {
                    //case Playback.Pause:
                    //    return;

                    //case Playback.Play:
                    //    return;

                    case Direction.Forward:
                        Forward();
                        return;

                    case Direction.Backward:
                        Back();
                        return;
                }
            }

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