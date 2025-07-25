using Utility.Interfaces.Exs;

namespace Utility.Models.Diagrams
{
    public class ObservableNode(IObservable<object> observable) : IResolvableNode
    {
        public IObservable<object> Connector { get; } = observable;
        //public IValueModel ValueModel { get; } = valueModel;

        public override bool Equals(object? obj)
        {
            if (obj is ObservableNode mNode)
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
