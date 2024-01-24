using System.ComponentModel;
using Utility.Objects;

namespace Utility.Nodes
{
    public record ByteValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<byte>(propertyDescriptor, Instance)
    {
    }
    
    public record NullableByteValue(PropertyDescriptor propertyDescriptor, object Instance) : NullablePropertyData<byte>(propertyDescriptor, Instance)
    {
    }
}
