using Utility.Interfaces.Exs;

namespace Utility.Models.Diagrams
{
    public class ObserverNode(IObserver<object> observer) : IResolvableNode
    {
        public IObserver<object> Connector { get; } = observer;

        public override bool Equals(object? obj)
        {
            if (obj is ObserverNode mNode)
                return Connector.Equals(mNode.Connector);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Connector.GetHashCode();
        }
    }

}
