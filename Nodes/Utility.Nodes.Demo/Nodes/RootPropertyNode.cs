using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodes.Demo.Infrastructure;
using Utility.Properties;

namespace Utility.Nodes.Demo
{
    public class RootPropertyNode : PropertyNode
    {
        public RootPropertyNode() : base(new PropertyData(Model, new RootDescriptor(Model)))
        {

        }

        static LedModel Model { get; } = new LedModel();
    }
}
