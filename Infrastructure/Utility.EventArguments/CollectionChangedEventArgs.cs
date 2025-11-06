using System.Collections;
using System.Collections.Generic;
using Utility.Enums;

namespace Utility.EventArguments
{
    public class CollectionChangedEventArgs : CollectionItemEventArgs
    {
        public CollectionChangedEventArgs(IEnumerable array, IReadOnlyCollection<object> changes, EventType eventType, object item, int index) : base(eventType, item, index)
        {
            Objects = array;
            Changes = changes;
        }

        public IEnumerable Objects { get; }
        public IReadOnlyCollection<object> Changes { get; }
    }
}