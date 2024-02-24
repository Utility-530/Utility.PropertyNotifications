using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Changes
{



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


}