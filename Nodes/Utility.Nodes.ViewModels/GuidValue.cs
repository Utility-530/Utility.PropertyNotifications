using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class GuidValue : BasePropertyObject, IValue<Guid>
    {


        public GuidValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public Guid Value
        {
            get => (Guid)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }


        object IValue.Value => Value;
    }

}
