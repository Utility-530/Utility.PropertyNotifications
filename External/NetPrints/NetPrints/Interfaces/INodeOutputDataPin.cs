using NetPrints.Core;
using NetPrints.Interfaces;

namespace NetPrints.Graph
{
    public interface INodeOutputDataPin : IDataPin, INodePin
    {
        IObservableCollection<INodeInputDataPin> OutgoingPins { get; }
    }
}