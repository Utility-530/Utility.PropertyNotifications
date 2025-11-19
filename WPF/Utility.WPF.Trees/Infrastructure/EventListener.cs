using Utility;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Reactives;
using Utility.WPF.Trees;

namespace Utility.WPF.Trees
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