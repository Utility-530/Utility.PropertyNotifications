using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utility.Interfaces.NonGeneric.Dependencies;

public interface IResolver
{
    object? Resolve(Type? serviceType, string? contract = null);

    ObservableCollection<object> ResolveMany(Type? serviceType, string? contract = null);
}