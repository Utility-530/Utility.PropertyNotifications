using NetPrints.Core;
using NetPrints.Interfaces;
using System;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Node that returns the default value of a type.
    /// </summary>
    [DataContract]
    public class DefaultNode : Node
    {
        /// <summary>
        /// Pin for the default value.
        /// </summary>
        public INodeOutputDataPin DefaultValuePin
        {
            get { return OutputDataPins[0]; }
        }

        /// <summary>
        /// Input type pin for the type to cast to.
        /// </summary>
        public INodeInputTypePin TypePin
        {
            get { return InputTypePins[0]; }
        }

        /// <summary>
        /// Type for the default value output. Inferred from input type pin.
        /// </summary>
        public IBaseType Type
        {
            get => TypePin.InferredType ?? TypeSpecifier.FromType<object>();
        }

        public DefaultNode(INodeGraph graph)
            : base(graph)
        {
            AddInputTypePin("Type");
            AddOutputDataPin("Default", Type);
        }

        protected override void OnInputTypeChanged(object sender, EventArgs eventArgs)
        {
            base.OnInputTypeChanged(sender, eventArgs);

            DefaultValuePin.PinType = Type;
        }

        public override string ToString()
        {
            return $"Default {Type.ShortName}";
        }
    }
}
