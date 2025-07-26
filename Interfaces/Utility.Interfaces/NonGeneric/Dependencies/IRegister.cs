using System;

namespace Utility.Interfaces.NonGeneric.Dependencies;

public interface IRegister
{
    bool Has(Type? serviceType, string? contract = null);

    void Register(Func<object?> factory, Type? serviceType, string? contract = null);

    void UnregisterCurrent(Type? serviceType, string? contract = null);

    void UnregisterAll(Type? serviceType, string? contract = null);

}
