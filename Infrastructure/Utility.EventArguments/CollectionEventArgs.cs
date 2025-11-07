using Utility.Enums;

namespace Utility.EventArguments
{
    public class CollectionEventArgs
    {
        public CollectionEventArgs(EventType eventType)
        {
            EventType = eventType;
        }

        public EventType EventType { get; }
    }
}