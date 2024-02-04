using Utility.Nodes.Reflections;
using Utility.Objects;
using Utility.Observables.Generic;

namespace Utility.Nodes
{
    public class MethodsNode : ReflectionNode
    {
        private MethodsData data;

        public MethodsNode(MethodsData propertyData)
        {
            this.data = propertyData;
        }

        public override object Data => data;

        public override async Task<object?> GetChildren()
        {
            var children = MethodExplorer.MethodInfos(data.Descriptor.PropertyType).ToArray();
            return children.Select(methodInfo => new MethodData(methodInfo, data.Instance)).ToArray();
        }

        public override string ToString()
        {
            return data.Name;
        }
    }
}

