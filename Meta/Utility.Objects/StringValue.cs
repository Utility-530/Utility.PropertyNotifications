using System.ComponentModel;
using System.Reflection;
using Utility.Helpers;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Objects;

namespace Utility.Nodes
{
    public record StringValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData<string>(propertyDescriptor, Instance)
    {
    }
}
