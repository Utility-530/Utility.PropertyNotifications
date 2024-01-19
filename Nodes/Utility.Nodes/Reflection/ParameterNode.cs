using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Objects;
using Utility.Reactive.Helpers;

namespace Utility.Nodes
{
    public class ParameterNode : Node
    {
        private ParameterData data;
        private readonly Lazy<object> _data;
        private bool flag;

        public ParameterNode(ParameterData propertyData)
        {
            this.data = propertyData;
            this._data = new Lazy<object>(() => ObjectConverter.ToValue(data.Instance, data.Descriptor));
        }

        public override object Data => _data.Value;

        public override IEquatable Key => null;

        public override async Task<object?> GetChildren()
        {
            flag = true;
            var inst = data.Descriptor.GetValue(data.Instance);
            var children = await ChildPropertyExplorer.Convert(inst, data.Descriptor);
            //if (inst.GetMethods().Any())
            //    return children.Select(a => new PropertyData(inst, a.Descriptor) as MemberData).Append(new MethodsData(inst, data.Descriptor)).ToArray();
            return children.Select(a => new PropertyData(inst, a.Descriptor)).ToArray();
        }

        public override string ToString()
        {
            return data.Descriptor.Name;
        }

        public override Node ToNode(object value)
        {
            if (value is PropertyData propertyData)
                return new PropertyNode(propertyData);
            //else if (value is MethodsData methodData)
            //{
            //    return new MethodsNode(methodData);
            //}
            else
                throw new Exception("34422 2!pod");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }

}
