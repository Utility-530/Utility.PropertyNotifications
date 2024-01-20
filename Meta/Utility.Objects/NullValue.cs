using System.ComponentModel;
using Utility.Interfaces.NonGeneric;
using Utility.Objects;

namespace Utility.Nodes
{
    public record NullValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData(propertyDescriptor, Instance), IValue
    {
        public object Value => null;
    }



}
