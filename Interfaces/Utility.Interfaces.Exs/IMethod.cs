using System.Collections.Generic;
using System.Reflection;

namespace Utility.Interfaces.Exs
{
    public interface IMethod
    {
        object? Instance { get; }
        MethodInfo MethodInfo { get; }
        string Name { get; }
        IReadOnlyCollection<ParameterInfo> Parameters { get; }

        //object? Execute(params object?[] objects);
    }
}