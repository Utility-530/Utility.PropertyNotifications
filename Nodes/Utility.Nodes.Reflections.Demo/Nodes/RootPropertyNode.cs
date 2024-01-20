using Utility.Nodes.Demo.Infrastructure;
using Utility.Objects;
using Utility.PropertyDescriptors;

namespace Utility.Nodes.Demo
{
    public class RootPropertyNode : PropertyNode
    {
        public RootPropertyNode() : base(new PropertyData(new RootDescriptor(Model), Model))
        {

        }

        static LedModel Model { get; } = new LedModel();
    }
}
