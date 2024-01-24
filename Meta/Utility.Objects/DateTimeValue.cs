using System.ComponentModel;
using Utility.Objects;

namespace Utility.Nodes
{
    public record DateTimeValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<DateTime>(propertyDescriptor, Instance)
    {
    }
    public record NullableDateTimeValue(PropertyDescriptor propertyDescriptor, object Instance) : NullablePropertyData<DateTime>(propertyDescriptor, Instance)
    {
    }
}
