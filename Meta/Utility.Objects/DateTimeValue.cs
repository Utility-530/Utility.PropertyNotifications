using System.ComponentModel;
using Utility.Objects;

namespace Utility.Nodes
{
    public record DateTimeValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<DateTime>(propertyDescriptor, Instance)
    {
    }
}
