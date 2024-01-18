using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class LongValue : BasePropertyObject, IValue<long>
    {
        public LongValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }
        public long Value
        {
            get => (long)Descriptor.GetValue(Instance); set
            {
                Descriptor.SetValue(Instance, value);
            }
        }

        object IValue.Value => Value;
    }

}
