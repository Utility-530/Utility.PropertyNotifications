using System.ComponentModel;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;
using Utility.Objects;
using Utility.Reactive.Helpers;
using System.Reflection;
using Utility.Helpers;
using Utility.PropertyDescriptors;
using System.Collections.Generic;
using Utility.PropertyDescriptors;

namespace Utility.Nodes
{
    public class PropertyNode : Node
    {
        private PropertyData data;
        private Lazy<object> _data;
        bool flag;

        public PropertyNode(PropertyData propertyData)
        {
            if (propertyData == null)
            {
            }
            this.data = propertyData;
            this._data = new Lazy<object>(() => ObjectConverter.ToValue(propertyData.Instance, propertyData.Descriptor));
        }

        public override object Data => _data.Value;

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
                return children.Select(a => new PropertyData(inst, a.Descriptor) as MemberData).Append(new MethodsData(inst, data.Descriptor)).ToArray();
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


    public record MemberData(object Instance, PropertyDescriptor Descriptor) : IValue
    {
        public object? Value => Instance;
    }

    public record PropertyData(object Instance, PropertyDescriptor Descriptor) : MemberData(Instance, Descriptor)
    {
    }

    public record MethodsData(object Instance, PropertyDescriptor Descriptor) : MemberData(Instance, Descriptor)
    {
    }

    public record MethodData(object Instance, MethodInfo Info) : IValue
    {
        public object? Value => Instance;
    }

    public record ParameterData(object Instance, ParameterDescriptor Descriptor) : IValue
    {
        public object? Value => Instance;
    }
}

