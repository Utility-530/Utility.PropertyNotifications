using System.ComponentModel;
using Utility.Objects;

namespace Utility.Nodes
{
    public record BooleanValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<bool>(propertyDescriptor, Instance)
    {   
    }   
    






}
