using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class DateTimeValue : BasePropertyObject, IValue<DateTime>
    {
        public DateTimeValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public DateTime Value
        {
            get => (DateTime)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }


        object IValue.Value => Value;
    }

}
