using NetPrints.Core;
using NetPrints.Interfaces;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NetPrints.Graph
{
    /// <summary>
    /// Node for awaiting tasks.
    /// </summary>
    [DataContract]
    public class AwaitNode : ExecNode
    {
        public override bool CanSetPure => true;

        public INodeInputDataPin TaskPin => InputDataPins[0];

        public INodeOutputDataPin ResultPin => OutputDataPins.FirstOrDefault();

        public AwaitNode(INodeGraph graph)
            : base(graph)
        {
            AddInputDataPin("Task", TypeSpecifier.FromType<Task>());
            SetupEvents();
            UpdateResultPin();
        }

        public override void OnMethodDeserialized()
        {
            base.OnMethodDeserialized();
            SetupEvents();
        }

        /// <summary>
        /// Sets up the task connection changed event which updates
        /// the result type.
        /// </summary>
        private void SetupEvents()
        {
            TaskPin.IncomingPinChanged += (pin, oldPin, newPin) => UpdateResultPin();
        }

        /// <summary>
        /// Updates the result pin's type depending on the incoming task's return type.
        /// </summary>
        private void UpdateResultPin()
        {
            // Check if the task returns a value and add or remove the result
            // pin depending on that.

            TypeSpecifier taskType = (TypeSpecifier)(TaskPin.IncomingPin?.PinType ?? TypeSpecifier.FromType<Task>());

            if (taskType.GenericArguments.Count > 0)
            {
                IBaseType returnType = taskType.GenericArguments[0];

                if (ResultPin != null)
                {
                    ResultPin.PinType = returnType;

                    // Disconnect all existing connections.
                    // Might want them to stay connected but that requires reflection
                    // to determine if the types are still compatible.
                    foreach (var outgoingPin in ResultPin.OutgoingPins)
                    {
                        GraphUtil.DisconnectOutputDataPin(ResultPin);
                    }
                }
                else
                {
                    AddOutputDataPin("Result", returnType);
                }
            }
            else
            {
                // Remove existing result pin if any
                if (ResultPin != null)
                {
                    GraphUtil.DisconnectOutputDataPin(ResultPin);
                    OutputDataPins.Remove(ResultPin);
                }
            }
        }
    }
}
