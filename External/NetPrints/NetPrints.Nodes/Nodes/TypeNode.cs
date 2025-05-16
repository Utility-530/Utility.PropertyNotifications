using NetPrints.Core;
using NetPrints.Interfaces;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Graph
{

    [DataContract]
    public class TypeNode : Node
    {
        [DataMember]
        public IBaseType Type
        {
            get;
            private set;
        }

        [DataMember]
        private ObservableValue<IBaseType> constructedType;

        public TypeNode(INodeGraph graph, IBaseType type)
            : base(graph)
        {
            Type = type;

            // Add type pins for each generic argument of the literal type
            // and monitor them for changes to reconstruct the actual pin types.
            if (Type is TypeSpecifier typeSpecifier)
            {
                foreach (var genericArg in typeSpecifier.GenericArguments.OfType<GenericType>())
                {
                    AddInputTypePin(genericArg.Name);
                }
            }

            constructedType = new ObservableValue<IBaseType>(GetConstructedOutputType());
            AddOutputTypePin("OutputType", constructedType);
        }

        protected override void OnInputTypeChanged(object sender, EventArgs eventArgs)
        {
            base.OnInputTypeChanged(sender, eventArgs);

            // Set the type of the output type pin by constructing
            // the type of this node with the input type pins.
            constructedType.Value = GetConstructedOutputType();
        }

        private IBaseType GetConstructedOutputType()
        {
            return GenericsHelper.ConstructWithTypePins(Type, InputTypePins);
        }

        public override string ToString()
        {
            return Type.ShortName;
        }
    }
}
