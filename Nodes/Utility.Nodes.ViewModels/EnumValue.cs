using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class EnumValue : BasePropertyObject, IValue<Enum>
    {


        public EnumValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public Enum Value
        {
            get => (Enum)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }

        }

        object IValue.Value => Value;
    }

}
