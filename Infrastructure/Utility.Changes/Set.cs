using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Changes
{

    public class Set<T> : ReadOnlyCollection<Change<T>>
    {
        public Set(IList<Change<T>> list) : base(list)
        {
        }

        public Set(Change<T> change) : base(new[] { change })
        {
        }
    }

    public class Set : ReadOnlyCollection<Change>
    {
        public Set(IEquatable key, IList<Change> list) : base(list)
        {
            Key = key;
        }
        public virtual IEquatable Key { get; }

        public Set(Change change) : base(new[] { change })
        {
        }
    }

    public record Change<T> : Change
    {
        public Change(T? tValue, Type type) : base(tValue, type)
        {
            Value = tValue;
        }

        public new T Value { get; }

        public new Set<T> ToChangeSet()
        {
            return new Set<T>(this);
        }

        public static Change<T> None => new(default, Type.None);

        public Change<TR> As<TR>() where TR : class
        {
            return new Change<TR>(Value as TR ?? throw new Exception("sd ssss"), Type);
        }
    }


}