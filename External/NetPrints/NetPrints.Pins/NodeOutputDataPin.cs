using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Pin which outputs a value. Can be connected to input data pins.
    /// </summary>
    [DataContract]
    public class NodeOutputDataPin : NodeDataPin, INodeOutputDataPin
    {
        /// <summary>
        /// Connected input data pins.
        /// </summary>
        [DataMember]
        public IObservableCollection<INodeInputDataPin> OutgoingPins { get; private set; }
            = new ObservableRangeCollection<INodeInputDataPin>();

        public NodeOutputDataPin(INode node, string name, IBaseType pinType)
            : base(node, name, pinType)
        {
        }
    }
}
