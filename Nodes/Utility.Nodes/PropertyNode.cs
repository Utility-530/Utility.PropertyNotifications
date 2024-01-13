using System.ComponentModel;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.Generic;
using Utility.Observables.NonGeneric;
using System.Runtime.CompilerServices;
using Utility.Objects;
using Utility.Reactive.Helpers;

namespace Utility.Nodes
{

    public class PropertyNode : Node
    {
        private PropertyData data;
        bool flag;

        public PropertyNode(PropertyData propertyData)
        {
            if (propertyData == null)
            {

            }
            this.data = propertyData;
        }

        public override object Data => ObjectConverter.ToValue(data.Instance, data.Descriptor);

        public override IEquatable Key => null;

        public override async Task<object?> GetChildren()
        {
            flag = true;
            var inst = data.Descriptor.GetValue(data.Instance);
            var children = await ChildPropertyExplorer.Convert(inst, data.Descriptor);
            return children.Select(a => new PropertyData(inst, a.Descriptor)).ToArray();
        }

        public override string ToString()
        {
            //return data.Name;
            return "";
        }

        public override Node ToNode(object value)
        {
            return new PropertyNode(value as PropertyData);
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }

    public record PropertyData(object Instance, PropertyDescriptor Descriptor) : IValue
    {
        public object? Value => Instance;
    }
}

