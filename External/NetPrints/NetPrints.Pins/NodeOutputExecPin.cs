using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{

    /// <summary>
    /// Pin which can be connected to an input execution pin to pass along execution.
    /// </summary>
    [DataContract]
    public class NodeOutputExecPin : NodeExecPin, INodeOutputExecPin
    {
        /// <summary>
        /// Called when the connected outgoing pin changed.
        /// </summary>
        public event OutputExecPinOutgoingPinChangedDelegate OutgoingPinChanged;

        /// <summary>
        /// Connected input execution pin. Null if not connected.
        /// Can trigger OutgoingPinChanged when set.
        /// </summary>
        [DataMember]
        public INodeInputExecPin OutgoingPin
        {
            get => outgoingPin;
            set
            {
                if (outgoingPin != value)
                {
                    var oldPin = outgoingPin;

                    outgoingPin = value;

                    OutgoingPinChanged?.Invoke(this, oldPin, outgoingPin);
                }
            }
        }

        private INodeInputExecPin outgoingPin;

        public NodeOutputExecPin(INode node, string name)
            : base(node, name)
        {
        }
    }
}
