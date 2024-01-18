using System.ComponentModel;
using System.Reflection;
using Utility.Helpers;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class StringValue : BasePropertyObject, IValue<string>
    {

        public StringValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {
        }

        public string Value { get => (string)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); } }

        object IValue.Value => Value;
    }

}
