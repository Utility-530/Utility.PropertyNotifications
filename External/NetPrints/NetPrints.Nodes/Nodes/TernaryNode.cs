using NetPrints.Core;
using NetPrints.Interfaces;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{
    /// <summary>
    /// Node representing a ternary operation.
    /// </summary>
    [DataContract]
    public class TernaryNode : ExecNode
    {
        public override bool CanSetPure
        {
            get => true;
        }

        /// <summary>
        /// Pin for the object to choose when the condition was true.
        /// </summary>
        public INodeInputDataPin TrueObjectPin
        {
            get { return InputDataPins[0]; }
        }

        /// <summary>
        /// Pin for the object to choose when the condition was false.
        /// </summary>
        public INodeInputDataPin FalseObjectPin
        {
            get { return InputDataPins[1]; }
        }

        /// <summary>
        /// Pin for the selection condition.
        /// </summary>
        public INodeInputDataPin ConditionPin
        {
            get { return InputDataPins[2]; }
        }

        /// <summary>
        /// Input type pin for the type to select.
        /// </summary>
        public INodeInputTypePin TypePin
        {
            get { return InputTypePins[0]; }
        }

        /// <summary>
        /// Pin that holds the selected object.
        /// </summary>
        public INodeOutputDataPin OutputObjectPin
        {
            get { return OutputDataPins[0]; }
        }

        /// <summary>
        /// Type to cast to. Inferred from input type pin.
        /// </summary>
        public IBaseType Type
        {
            get => TypePin.InferredType ?? TypeSpecifier.FromType<object>();
        }

        public TernaryNode(INodeGraph graph)
            : base(graph)
        {
            AddInputTypePin("Type");
            AddInputDataPin("True", TypeSpecifier.FromType<object>());
            AddInputDataPin("False", TypeSpecifier.FromType<object>());
            AddInputDataPin("Condition", TypeSpecifier.FromType<bool>());
            AddOutputDataPin("Output", Type);
        }

        protected override void OnInputTypeChanged(object sender, EventArgs eventArgs)
        {
            base.OnInputTypeChanged(sender, eventArgs);

            TrueObjectPin.PinType = Type;
            FalseObjectPin.PinType = Type;
            OutputObjectPin.PinType = Type;
        }

        public override string ToString()
        {
            return $"Ternary {Type.ShortName}";
        }
    }
}
