using Utility.Enums;

namespace Utility.EventArguments
{
    public class CollectionItemEventArgs : CollectionEventArgs
    {
        public CollectionItemEventArgs(EventType eventType, object item, int index) : base(eventType)
        {
            Item = item;
            Index = index;
        }

        public object Item { get; }
        public int Index { get; }
    }
}