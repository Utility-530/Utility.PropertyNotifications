using Utility.Nodes.Demo.Infrastructure;
using Utility.PropertyDescriptors;

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
