using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;
using Utility.Objects;
using Utility.Reactive.Helpers;

namespace Utility.Nodes
{
    public class MethodsNode : Node
    {
        private MethodsData data;
        bool flag;

        public MethodsNode(MethodsData propertyData)
        {
            if (propertyData == null)
            {

            }
            this.data = propertyData;
        }

        public override object Data => data;

        public override IEquatable Key => null;

        public override async Task<object?> GetChildren()
        {
            flag = true;         
            var children = MethodExplorer.MethodInfos(data.Descriptor);
            return children.Select(methodInfo => new MethodData(methodInfo, data.Instance)).ToArray();
        }

        public override string ToString()
        {
            //return data.Name;
            return "";
        }

        public override Node ToNode(object value)
        {
            if (value is MethodData methodData)
                return new MethodNode(methodData);
            throw new Exception("34456566622 2!pod");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

