using System.ComponentModel;
using Utility.Objects;

namespace Utility.Nodes
{
    public record ObjectValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<object>(propertyDescriptor, Instance)
    {
    }


}
