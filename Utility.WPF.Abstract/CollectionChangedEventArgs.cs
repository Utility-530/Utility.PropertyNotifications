using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Utility.Enums;

namespace UtilityWpf.Abstract
{
    public class CollectionEventArgs : RoutedEventArgs
    {
        public CollectionEventArgs(EventType eventType, RoutedEvent @event) : base(@event)
        {
            EventType = eventType;
        }

        public EventType EventType { get; }
    }

    public class CollectionItemEventArgs : CollectionEventArgs
    {
        public CollectionItemEventArgs(EventType eventType, object? item, int index, RoutedEvent @event) : base(eventType, @event)
        {
            Item = item;
            Index = index;
        }

        public object? Item { get; }
        public int Index { get; }
    }

    public class CollectionItemChangedEventArgs : CollectionItemEventArgs
    {
        public CollectionItemChangedEventArgs(IEnumerable array, IReadOnlyCollection<object> changes, EventType eventType, object? item, int index, RoutedEvent @event) : base(eventType, item, index, @event)
        {
            Objects = array;
            Changes = changes;
        }

        public IEnumerable Objects { get; }
        public IReadOnlyCollection<object> Changes { get; }
    }

    public class MovementEventArgs : CollectionItemEventArgs
    {
        public MovementEventArgs(IReadOnlyCollection<IndexedObject> array, IReadOnlyCollection<IndexedObject> changes, EventType eventType, object? item, int index, RoutedEvent @event) : base(eventType, item, index, @event)
        {
            Objects = array;
            Changes = changes;
        }

        public IReadOnlyCollection<IndexedObject> Objects { get; }
        public IReadOnlyCollection<IndexedObject> Changes { get; }
    }

    public class IndexedObject
    {
        public IndexedObject(object @object, int index, int oldIndex)
        {
            Object = @object;
            Index = index;
            OldIndex = oldIndex;
        }

        public int Index { get; set; }
        public int OldIndex { get; }
        public object Object { get; }
    }

    public delegate void CollectionChangedEventHandler(object sender, CollectionEventArgs e);
}