using System;
using System.Collections.Generic;

namespace Utility.Interfaces.NonGeneric.Dependencies;


public interface IResolver
{
    object? Resolve(Type? serviceType, string? contract = null);

    IEnumerable<object> ResolveMany(Type? serviceType, string? contract = null);
}
