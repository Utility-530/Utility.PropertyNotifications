using System.ComponentModel;
using Utility.Objects;

namespace Utility.Nodes
{
    public record LongValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<long>(propertyDescriptor, Instance)
    {
    }
    
    public record NullableLongValue(PropertyDescriptor propertyDescriptor, object Instance) : NullablePropertyData<long>(propertyDescriptor, Instance)
    {
    }

}
