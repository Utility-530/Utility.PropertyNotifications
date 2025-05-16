using NetPrints.Core;
using System;
using System.Runtime.Serialization;
using System.Linq;
using NetPrints.Interfaces;

namespace NetPrints.Graph
{
    /// <summary>
    /// Node representing the initial execution node of a method.
    /// </summary>
    [DataContract]
    public class MethodEntryNode : ExecutionEntryNode, IMethodEntryNode
    {
        public MethodEntryNode(IMethodGraph graph)
            : base(graph)
        {
            AddOutputExecPin("Exec");
        }

        protected override void OnInputTypeChanged(object sender, EventArgs eventArgs)
        {
            base.OnInputTypeChanged(sender, eventArgs);

            for (int i = 0; i < InputTypePins.Count; i++)
            {
                OutputDataPins[i].PinType = InputTypePins[i].InferredType ?? TypeSpecifier.FromType<object>();
            }
        }

        public override string ToString()
        {
            return $"{MethodGraph.Name} Entry";
        }

        public void AddArgument()
        {
            int argIndex = OutputDataPins.Count;
            AddOutputDataPin($"Input{argIndex}", TypeSpecifier.FromType<object>());
            AddInputTypePin($"Input{argIndex}Type");
        }

        public void RemoveArgument()
        {
            if (OutputDataPins.Count > 0)
            {
                INodeOutputDataPin odpToRemove = OutputDataPins.Last();
                INodeInputTypePin itpToRemove = InputTypePins.Last();

                GraphUtil.DisconnectOutputDataPin(odpToRemove);
                GraphUtil.DisconnectInputTypePin(itpToRemove);

                OutputDataPins.Remove(odpToRemove);
                InputTypePins.Remove(itpToRemove);
            }
        }

        public void AddGenericArgument()
        {
            string name = $"T{OutputTypePins.Count}";
            AddOutputTypePin(name, new GenericType(name));
        }

        public void RemoveGenericArgument()
        {
            if (OutputTypePins.Count > 0)
            {
                INodeOutputTypePin otpToRemove = OutputTypePins.Last();

                GraphUtil.DisconnectOutputTypePin(otpToRemove);

                OutputTypePins.Remove(otpToRemove);
            }
        }
    }
}
