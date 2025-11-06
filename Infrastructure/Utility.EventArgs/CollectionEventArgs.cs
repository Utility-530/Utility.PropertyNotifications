using Utility.Enums;

namespace Utility.EventArgs
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