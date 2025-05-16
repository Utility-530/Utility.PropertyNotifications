using NetPrints.Core;
using NetPrints.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NetPrints.Graph
{
    [DataContract]
    public class ClassReturnNode : Node
    {
        public INodeInputTypePin SuperTypePin
        {
            get => InputTypePins[0];
        }

        public IEnumerable<INodeInputTypePin> InterfacePins
        {
            get => InputTypePins.Skip(1);
        }

        public ClassReturnNode(IClassGraph graph)
            : base(graph)
        {
            AddInputTypePin("BaseType");
        }

        public void AddInterfacePin()
        {
            AddInputTypePin($"Interface{InputTypePins.Count}");
        }

        public void RemoveInterfacePin()
        {
            var interfacePin = InterfacePins.LastOrDefault();

            if (interfacePin != null)
            {
                GraphUtil.DisconnectInputTypePin(interfacePin);
                InputTypePins.Remove(interfacePin);
            }
        }
    }
}
