using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Objects;

namespace Utility.Nodes
{
    public record IntegerValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<int>(propertyDescriptor, Instance)
    {
    }
        public record NullableIntegerValue(PropertyDescriptor propertyDescriptor, object Instance) : NullablePropertyData<int>(propertyDescriptor, Instance)
    {
    }

}
