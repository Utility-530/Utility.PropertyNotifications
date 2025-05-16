using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{

    /// <summary>
    /// Pin which can receive types.
    /// </summary>
    [DataContract]
    public class NodeInputTypePin : NodeTypePin, INodeInputTypePin
    {
        private INodeOutputTypePin incomingPin;

        /// <summary>
        /// Called when the node's incoming pin changed.
        /// </summary>
        public event InputTypePinIncomingPinChangedDelegate IncomingPinChanged;

        /// <summary>
        /// Incoming type pin for this pin. Null when not connected.
        /// Can trigger IncomingPinChanged when set.
        /// </summary>
        [DataMember]
        public INodeOutputTypePin IncomingPin
        {
            get => incomingPin;
            set
            {
                if (incomingPin != value)
                {
                    var oldPin = incomingPin;

                    incomingPin = value;

                    IncomingPinChanged?.Invoke(this, oldPin, incomingPin);
                }
            }
        }

        public override IBaseType InferredType
        {
            get => IncomingPin?.InferredType;
            set => IncomingPin.InferredType = value;
        }


        public NodeInputTypePin(INode node, string name)
            : base(node, name)
        {
        }
    }
}
