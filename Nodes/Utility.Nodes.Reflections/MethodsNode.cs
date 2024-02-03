using Utility.Models;
using Utility.Nodes.Reflections;
using Utility.Observables.Generic;
using Utility.Trees.Abstractions;

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

        public override async Task<object?> GetChildren()
        {
            flag = true;
            var children = MethodExplorer.MethodInfos(data.Descriptor.PropertyType).ToArray();
            return children.Select(methodInfo => new MethodData(methodInfo, data.Instance)).ToArray();
        }

        public override string ToString()
        {
            //return data.Name;
            return "";
        }

        public override async Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is MethodData { Info.Name: { } name } methodData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, name);
                    return new MethodNode(methodData) { Key = new Key(_guid, name, null) };
                }
                else
                    throw new Exception("f 32676 443opppp");
            }
            throw new Exception("34456566622 2!pod");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

