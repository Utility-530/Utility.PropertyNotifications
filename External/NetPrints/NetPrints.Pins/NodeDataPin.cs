using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{


    /// <summary>
    /// Abstract class for data pins.
    /// </summary>
    [DataContract]
    public abstract class NodeDataPin : NodePin
    {
        /// <summary>
        /// Specifier for the type of this data pin.
        /// </summary>
        [DataMember]
        public IBaseType PinType { get; set; }

        protected NodeDataPin(INode node, string name, IBaseType pinType)
            : base(node, name)
        {
            PinType = pinType;
        }

        public override string ToString()
        {
            return $"{Name}: {PinType.ShortName}";
        }
    }
}
