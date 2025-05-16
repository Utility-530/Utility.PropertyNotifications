using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Abstract class for type pins.
    /// </summary>
    [DataContract]
    public abstract class NodeTypePin : NodePin
    {
        public abstract IBaseType InferredType
        {
            get;
            set;
        }

        protected NodeTypePin(INode node, string name)
            : base(node, name)
        {
        }
    }
}
