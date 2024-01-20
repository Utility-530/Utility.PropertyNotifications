using System.ComponentModel;
using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyDescriptors;

namespace Utility.Nodes
{
    public record MemberData(PropertyDescriptor Descriptor, object Instance) 
    {

    }

    //public record PropertyData(object Instance, PropertyDescriptor Descriptor) : MemberData(Instance, Descriptor)
    //{
    //}

    public record MethodsData(PropertyDescriptor Descriptor, object Instance) : MemberData(Descriptor, Instance)
    {
    }

    public record MethodData(MethodInfo Info, object Instance) 
    {
    }

    public record ParameterData(ParameterDescriptor Descriptor, object Instance) 
    {
    }
}
