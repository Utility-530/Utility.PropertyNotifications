using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class IntegerValue : BasePropertyObject, IValue<int>
    {
        public IntegerValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }
        public int Value
        {
            get => (int)Descriptor.GetValue(Instance); set
            {
                Descriptor.SetValue(Instance, value);
            }
        }

        object IValue.Value => Value;
    }

}
