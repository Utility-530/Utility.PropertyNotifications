using Utility.Interfaces.NonGeneric;
using Utility.Reactives;

namespace Utility.Trees.Demo.Filters
{
    public class EventListener : ReplayModel<IEvent>, IEventListener
    {

        public static EventListener Instance { get; } = new();

        public void Send(IEvent @event)
        {
            OnNext(@event);
        }
    }
}
