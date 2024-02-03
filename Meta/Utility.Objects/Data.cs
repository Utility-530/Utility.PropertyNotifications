using System.ComponentModel;
using System.Reflection;
using Utility.Objects;
using Utility.PropertyDescriptors;

namespace Utility.Nodes
{

    public record MethodsData(PropertyDescriptor Descriptor, object Instance) : PropertyData(Descriptor, Instance)
    {
    }

    public record MethodData(MethodInfo Info, object Instance) 
    {
    }
}
