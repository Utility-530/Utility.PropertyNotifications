using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Utility.Interfaces.Exs
{
    public interface IMethodFactory
    {
        IEnumerable<MethodInfo> Methods { get; }
    }

    public interface IPropertyFactory
    {
        IEnumerable<PropertyInfo> Properties { get; }
    }

    public interface INodePropertyFactory
    {
        IEnumerable<PropertyInfo> Properties(IEnumerable<Type> types);
    }

}
