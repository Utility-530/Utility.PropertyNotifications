using System.ComponentModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces
{
    public interface IPropertyDescriptor
    {
        PropertyDescriptor Descriptor { get; }
    }

    public interface IValueDescriptor : IDescriptor, IGet, ISet, IValue, ISetValue
    {

    }

}