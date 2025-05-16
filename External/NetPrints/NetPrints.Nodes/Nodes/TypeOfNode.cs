using NetPrints.Core;
using NetPrints.Interfaces;
using System;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Node representing a literal value.
    /// </summary>
    [DataContract]
    public class TypeOfNode : Node
    {
        /// <summary>
        /// Output data pin for the Type value.
        /// </summary>
        public INodeOutputDataPin TypePin
        {
            get { return OutputDataPins[0]; }
        }

        /// <summary>
        /// Input type pin for the Type value.
        /// </summary>
        public INodeInputTypePin InputTypePin
        {
            get { return InputTypePins[0]; }
        }

        public TypeOfNode(INodeGraph graph)
            : base(graph)
        {
            AddInputTypePin("Type");
            AddOutputDataPin("Type", TypeSpecifier.FromType<Type>());
        }

        public override string ToString()
        {
            return $"Type Of";
        }
    }
}
