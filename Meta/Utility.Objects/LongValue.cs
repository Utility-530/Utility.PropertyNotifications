using System.ComponentModel;
using Utility.Objects;

namespace Utility.Nodes
{
    public record LongValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<DateTime>(propertyDescriptor, Instance)
    {
    }

}
