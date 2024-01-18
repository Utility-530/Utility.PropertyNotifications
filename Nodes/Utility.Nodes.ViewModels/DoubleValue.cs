using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class DoubleValue : BasePropertyObject, IValue<double>
    {

        public DoubleValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {


        }

        public double Value
        {
            get => (double)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }


        object IValue.Value => Value;
    }

}
