using NetPrints.Core;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    [DataContract]
    public class TypeReturnNode : Node, ITypeReturnNode
    {
        public INodeInputTypePin TypePin => InputTypePins[0];

        public ITypeSpecifier? InferredType { get; set; }

        public TypeReturnNode(ITypeGraph graph)
            : base(graph)
        {
            AddInputTypePin("Type");
        }
    }
}
