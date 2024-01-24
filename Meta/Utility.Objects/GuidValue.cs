using System.ComponentModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Objects;

namespace Utility.Nodes
{
    public record GuidValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<Guid>(propertyDescriptor, Instance)
    {
    }
    
    public record NullableGuidValue(PropertyDescriptor propertyDescriptor, object Instance) : NullablePropertyData<Guid>(propertyDescriptor, Instance)
    {
    }

}
