using System.Collections;
using System.Collections.ObjectModel;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;


namespace Utility.PropertyTrees.Infrastructure
{
    public class History : BaseObject, IHistory
    {
        private readonly Collection<Order> past = new(), present = new(), future = new();

        public History()
        {
        }

        public Guid Guid => Guid.Parse("58a13c8e-d9f4-4117-ab37-dacd58c45340");

        public IEnumerable Past => past;

        public IEnumerable Present => present;

        public IEnumerable Future => future;

        public override Key Key => new(default, nameof(History), typeof(History));

        public void OnNext(object value)
        {
            if (value is Direction direction)
            {
                switch (direction)
                {
                    case Direction.Forward:
                        Broadcast(new ChangeSet(this.Key, Forward().ToList()));
                        return;
                    case Direction.Backward:
                        Broadcast(new ChangeSet(this.Key, Back().ToList()));
                        return;
                }
            }

            if (isDirty)
            {
                future.Clear();
            }
            if (value is not Order order)
            {
                throw new Exception("rfe w3");
            }

            if (future.Any(a => a.Key == order.Key && a.Access == order.Access && a.Value == order.Value))
            {
                return;
            }

            future.Add(order);
            Broadcast(new ChangeSet(this.Key, new[] { new Change(new HistoryOrder(Enums.History.Future, order), ChangeType.Add, future.Count) }));
        }


        private IEnumerable<Change> Forward()
        {
            var order = future[0];
            if (present.Count > 0)
            {
                yield return new Change(new HistoryOrder(Enums.History.Past, present[0]), ChangeType.Add, past.Count);
                past.Add(present[0]);
            }
            if (present.Count > 0)
            {
                yield return new Change(new HistoryOrder(Enums.History.Present, present[0]), ChangeType.Remove, present.Count);
                present.RemoveAt(0);

            }
            yield return new Change(new HistoryOrder(Enums.History.Present, order), ChangeType.Add, present.Count);
            yield return new Change(new HistoryOrder(Enums.History.Future, order), ChangeType.Remove, future.Count);
            present.Add(order);
            future.Remove(order);
        }

        private bool isDirty;

        private IEnumerable<Change> Back()
        {
            isDirty = true;
            var order = past[^1];
            //if (past.Any())
            if (present.Count > 0)
            {
                yield return new Change(new HistoryOrder(Enums.History.Future, present[0]), ChangeType.Remove, future.Count);
                future.Insert(0, present[0]);
            }
            if (present.Count > 0)
            {
                yield return new Change(new HistoryOrder(Enums.History.Present, present[0]), ChangeType.Remove, present.Count);
                present.RemoveAt(0);
            }

            yield return new Change(new HistoryOrder(Enums.History.Present, order), ChangeType.Add, present.Count);
            yield return new Change(new HistoryOrder(Enums.History.Past, order), ChangeType.Remove, past.Count);
            present.Add(order);
            past.Remove(order);
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

    public record HistoryOrder(Enums.History History, Order Order) : IEquatable
    {
        public bool Equals(IEquatable? other)
        {
            return (other as HistoryOrder)?.Order.Key == Order.Key;
        }
    }
}