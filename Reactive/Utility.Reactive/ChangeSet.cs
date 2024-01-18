using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public enum ChangeType
    {
        None, Add, Remove, Update, Reset
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

    public record Change<T> : Change
    {
        public Change(T? tValue, ChangeType type) : base(tValue, type)
        {
            Value = tValue;
        }

        public new T Value { get; }

        public new ChangeSet<T> ToChangeSet()
        {
            return new ChangeSet<T>(this);
        }

        public static Change<T> None => new(default, ChangeType.None);

        public Change<TR> As<TR>() where TR : class
        {
            return new Change<TR>(Value as TR ?? throw new Exception("sd ssss"), Type);
        }
    }

    public record Change
    {
        public Change(object value, ChangeType type, int? count = default)
        {
            Value = value;
            Type = type;
            Count = count;
        }

        public object Value { get; }

        public ChangeType Type { get; init; }
        public int? Count { get; }

        public ChangeSet ToChangeSet()
        {
            return new ChangeSet(this);
        }
    }
}