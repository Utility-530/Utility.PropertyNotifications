using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Reflections;
using Utility.Objects;
using Utility.Reactive.Helpers;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class ParameterNode : Node
    {
        private readonly ParameterData data;
        private bool flag;

        public ParameterNode(ParameterData propertyData)
        {
            this.data = propertyData;
        }

        public override object Data => data;

        public override async Task<object?> GetChildren()
        {
            flag = true;
            var inst = data.Descriptor.GetValue(data.Instance);
            var children = await ChildPropertyExplorer.Convert(inst, data.Descriptor);
            return children.Select(a => ObjectConverter.ToValue(inst, a.Descriptor)).ToArray();
        }

        public override async Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is PropertyData { Descriptor.Name: { } name } propertyData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, name);
                    return new PropertyNode(propertyData) { Key = new Key(_guid, name, propertyData.Type) };
                }
                else
                    throw new Exception("f 32443opppp");
            }
            else
                throw new Exception("34422 2!pod");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
        public override string ToString()
        {
            return data.Descriptor.Name;
        }
    }
}
