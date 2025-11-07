using System.Collections.Generic;
using System.Reflection;

namespace Utility.Interfaces.Methods
{
    public interface IMethod
    {
        string Name { get; }
        object? Instance { get; }
        MethodInfo MethodInfo { get; }
        IReadOnlyCollection<ParameterInfo> Parameters { get; }
    }
}