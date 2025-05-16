using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Pin which outputs a type. Can be connected to input type pins.
    /// </summary>
    [DataContract]
    public class NodeOutputTypePin : NodeTypePin, INodeOutputTypePin
    {

        [DataMember]
        private IBaseType outputType;

        /// <summary>
        /// Connected input data pins.
        /// </summary>
        [DataMember]
        public IObservableCollection<INodeInputTypePin> OutgoingPins { get; private set; }
            = new ObservableRangeCollection<INodeInputTypePin>();

        public override IBaseType InferredType
        {
            get => outputType;
            set=> outputType = value;
        }


        public NodeOutputTypePin(INode node, string name, ObservableValue<IBaseType> outputType)
            : base(node, name)
        {
            this.outputType = outputType.Value;
        }

        public override string ToString()
        {
            return outputType.ShortName ?? "None";
        }
    }
}
