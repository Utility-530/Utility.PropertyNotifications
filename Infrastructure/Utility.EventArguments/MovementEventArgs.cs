using System.Collections.Generic;
using Utility.Enums;

namespace Utility.EventArguments
{
    public class MovementEventArgs : CollectionItemEventArgs
    {
        public MovementEventArgs(IReadOnlyCollection<IndexedObject> array, IReadOnlyCollection<IndexedObject> changes, EventType eventType, object item, int index) : base(eventType, item, index)
        {
            Objects = array;
            Changes = changes;
        }

        public IReadOnlyCollection<IndexedObject> Objects { get; }
        public IReadOnlyCollection<IndexedObject> Changes { get; }
    }
}