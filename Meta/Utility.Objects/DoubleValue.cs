using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Objects;

namespace Utility.Nodes
{
    public record DoubleValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<double>(propertyDescriptor, Instance)
    {
    }
    
    public record NullableDoubleValue(PropertyDescriptor propertyDescriptor, object Instance) : NullablePropertyData<double>(propertyDescriptor, Instance)
    {
    }


}
