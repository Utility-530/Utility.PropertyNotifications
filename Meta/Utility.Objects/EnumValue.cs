using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Objects;

namespace Utility.Nodes
{
    public record EnumValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<Enum>(propertyDescriptor, Instance)
    {
    }

    public record NullableEnumValue(PropertyDescriptor propertyDescriptor, object Instance) : NullablePropertyData<Enum>(propertyDescriptor, Instance)
    {
    }
}
