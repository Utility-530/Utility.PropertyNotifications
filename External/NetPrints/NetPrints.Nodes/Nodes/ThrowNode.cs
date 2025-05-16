using NetPrints.Core;
using NetPrints.Interfaces;
using System;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Node representing an exception throw.
    /// </summary>
    [DataContract]
    public class ThrowNode : Node
    {
        /// <summary>
        /// Pin for the exception to throw.
        /// </summary>
        public INodeInputDataPin ExceptionPin
        {
            get { return InputDataPins[0]; }
        }

        public ThrowNode(INodeGraph graph)
            : base(graph)
        {
            AddInputExecPin("Exec");
            AddInputDataPin("Exception", TypeSpecifier.FromType<Exception>());
        }

        public override string ToString()
        {
            return $"Throw Exception";
        }
    }
}
