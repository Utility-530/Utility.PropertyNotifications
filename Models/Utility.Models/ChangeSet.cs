using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public enum ChangeType
    {
        Add, Remove, Update
    }

    public class ChangeSet<T> : ReadOnlyCollection<Change<T>>
    {
        public ChangeSet(IList<Change<T>> list) : base(list)
        {
        }

        public ChangeSet(Change<T> change) : base(new[] { change })
        {
        }
    }

    public class ChangeSet : ReadOnlyCollection<Change>
    {
        public ChangeSet(IEquatable key, IList<Change> list) : base(list)
        {
            Key = key;
        }
        public virtual IEquatable Key { get; }

        public ChangeSet(Change change) : base(new[] { change })
        {
        }
    }

    public class Change<T>
    {
        public Change(T value, ChangeType type)
        {
            Value = value;
            Type = type;
        }

        public Change(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public ChangeType Type { get; init; }

        public ChangeSet<T> ToChangeSet()
        {
            return new ChangeSet<T>(this);
        }
    }

    public class Change
    {
        public Change(IEquatable value, ChangeType type, int? count = default)
        {
            Value = value;
            Type = type;
            Count = count;
        }

        public IEquatable Value { get; }

        public ChangeType Type { get; init; }
        public int? Count { get; }

        public ChangeSet ToChangeSet()
        {
            return new ChangeSet(this);
        }
    }
}