using System.ComponentModel;
using Utility.Interfaces.NonGeneric;
using Utility.Objects;

namespace Utility.Nodes
{
    public record ObjectValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<object>(propertyDescriptor, Instance)
    {
    }


}
