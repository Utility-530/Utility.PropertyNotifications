using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class BooleanValue : BasePropertyObject, IValue<bool>
    {

        public BooleanValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {


        }

        public bool Value
        {
            get => (bool)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }

        object IValue.Value => Value;
    }

}
