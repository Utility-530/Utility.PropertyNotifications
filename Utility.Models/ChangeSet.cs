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
        public ChangeSet(IList<Change> list) : base(list)
        {
        }

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

        public Change(Key value, ChangeType type)
        {
            Value = value;
            Type = type;
        }


        public Key Value { get; }

        public ChangeType Type { get; init; }
  

        public ChangeSet ToChangeSet()
        {
            return new ChangeSet(this);
        }
    }

}
