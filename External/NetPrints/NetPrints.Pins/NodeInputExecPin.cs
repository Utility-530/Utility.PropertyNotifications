using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Pin that can be connected to output execution pins to receive execution.
    /// </summary>
    [DataContract]
    public class NodeInputExecPin : NodeExecPin, INodeInputExecPin
    {
        /// <summary>
        /// Output execution pins connected to this pin.
        /// </summary>
        [DataMember]
        public IList<INodeOutputExecPin> IncomingPins { get; private set; } =
            new ObservableRangeCollection<INodeOutputExecPin>();

        public NodeInputExecPin(INode node, string name)
            : base(node, name)
        {
        }
    }
}
