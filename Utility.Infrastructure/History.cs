using System.Collections;
using System.Collections.ObjectModel;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Infrastructure.Abstractions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using h = Utility.Enums.History;

namespace Utility.PropertyTrees.Infrastructure
{
    public class History : BaseObject
    {
        private bool isDirty;
        private readonly Collection<object> past = new(), present = new(), future = new();

        public History()
        {
        }

        public Guid Guid => Guid.Parse("58a13c8e-d9f4-4117-ab37-dacd58c45340");

        public IEnumerable Past => past;

        public IEnumerable Present => present;

        public IEnumerable Future => future;

        public override Key Key => new(Guid, nameof(History), typeof(History));

        public override bool OnNext(object value)
        {
            if (value is Direction direction)
            {
                switch (direction)
                {
                    case Direction.Forward:
                        Broadcast(new ChangeSet(this.Key, Forward().ToList()));
                        return true;
                    case Direction.Backward:
                        Broadcast(new ChangeSet(this.Key, Back().ToList()));
                        return true;
                }
            }

            if (isDirty)
            {
                future.Clear();
            }

            if (future.Any(a => a.Equals(value)))
            {
                return false;
            }

            future.Add(value);
            Broadcast(new ChangeSet(this.Key, new[] { new Change(new HistoryOrder(h.Future, value), ChangeType.Add, future.Count) }));
            return true;
            IEnumerable<Change> Forward()
            {
                if (future.Any() == false)
                    yield break;
                var order = future[0];
                if (present.Count > 0)
                {
                    yield return new Change(new HistoryOrder(h.Past, present[0]), ChangeType.Add, past.Count);
                    past.Add(present[0]);
                }
                if (present.Count > 0)
                {
                    yield return new Change(new HistoryOrder(h.Present, present[0]), ChangeType.Remove, present.Count);
                    present.RemoveAt(0);

                }
                yield return new Change(new HistoryOrder(h.Present, order), ChangeType.Add, present.Count);
                yield return new Change(new HistoryOrder(h.Future, order), ChangeType.Remove, future.Count);
                present.Add(order);
                future.Remove(order);
            }

            IEnumerable<Change> Back()
            {
                isDirty = true;
                var order = past[^1];
                if (present.Count > 0)
                {
                    yield return new Change(new HistoryOrder(h.Future, present[0]), ChangeType.Remove, future.Count);
                    future.Insert(0, present[0]);
                }
                if (present.Count > 0)
                {
                    yield return new Change(new HistoryOrder(h.Present, present[0]), ChangeType.Remove, present.Count);
                    present.RemoveAt(0);
                }

                yield return new Change(new HistoryOrder(h.Present, order), ChangeType.Add, present.Count);
                yield return new Change(new HistoryOrder(h.Past, order), ChangeType.Remove, past.Count);
                present.Add(order);
                past.Remove(order);
            }
        }
    }

    public record HistoryOrder(h History, object Order) : IEquatable
    {
        public bool Equals(IEquatable? other)
        {
            return (other as HistoryOrder)?.Order.Equals(Order) == true;
        }
    }
}