using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;
using Utility.Objects;
using Utility.Reactive.Helpers;
using Utility.Helpers;
using Utility.PropertyDescriptors;

namespace Utility.Nodes
{
    public class PropertyNode : Node
    {
        private PropertyData data;
        bool flag;

        public PropertyNode(PropertyData propertyData)
        {
            if (propertyData.Descriptor == null)
            {
            }
            this.data = propertyData;
        }

        public override object Data => data;

        public override IEquatable Key => null;

        public override async Task<object?> GetChildren()
        {
            flag = true;
            var inst = data.Descriptor.GetValue(data.Instance);
            if (inst == null)
            {
                return Array.Empty<object>();
            }
            var children = await ChildPropertyExplorer.Convert(inst, data.Descriptor);
        
            if (data.Descriptor.IsValueOrStringProperty() ==false && MethodExplorer.MethodInfos(data.Descriptor).Any())
                return children.Select(a => ObjectConverter.ToValue(inst, a.Descriptor) as MemberData).Append(new MethodsData(data.Descriptor, inst)).ToArray();
            return children.Select(a => ObjectConverter.ToValue(inst, a.Descriptor)).ToArray();
        }

        public override string ToString()
        {
            return data.Descriptor.Name;
        }

        public override Node ToNode(object value)
        {
            if (value is PropertyData propertyData)
                return new PropertyNode(propertyData);
            else if (value is MethodsData methodData)
            {
                return new MethodsNode(methodData);
            }
            else
                throw new Exception("34422 2!pod");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

