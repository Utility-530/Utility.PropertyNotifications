using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public enum ChangeType
    {
        Add, Remove, Update
    }

    public enum Source
    {
        Internal, External
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

    public class Change<T>
    {

        public Change(T value, ChangeType type, Source source)
        {
            Value = value;
            Type = type;
            Source = source;
        }
        public Change(T value)
        {
            Value = value;
        }

        public T Value { get; }
        public ChangeType Type { get; init; }
        public Source Source { get; init; }

        public ChangeSet<T> ToChangeSet()
        {
            return new ChangeSet<T>(this);
        }
    }

}
