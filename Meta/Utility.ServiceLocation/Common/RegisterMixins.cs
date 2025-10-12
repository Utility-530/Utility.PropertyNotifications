// Copyright (c) 2025 ReactiveUI. All rights reserved.
// Licensed to ReactiveUI under one or more agreements.
// ReactiveUI licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Utility.Interfaces.NonGeneric.Dependencies;

namespace Utility.ServiceLocation;

/// <summary>
/// Resolver Mixins.
/// </summary>
public static class RegisterMixins
{
    /// <summary>
    /// Registers a factory for the given <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterAnd<T>(this IRegister resolver, string? contract = null)
        where T : new()
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        resolver.Register(() => new T(), typeof(T), contract);
        return resolver;
    }

    /// <summary>
    /// Registers a factory for the given <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="factory">A factory method for generating a object of the specified type.</param>
    /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterAnd<T>(this IRegister resolver, Func<T> factory, string? contract = null)
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));
        factory.ThrowArgumentNullExceptionIfNull(nameof(factory));

        resolver.Register(() => factory()!, typeof(T), contract);
        return resolver;
    }

    /// <summary>
    /// Registers a factory for the given <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="TAs">The type to register as.</typeparam>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterAnd<TAs, T>(this IRegister resolver, string? contract = null)
        where T : new()
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        resolver.Register(() => new T(), typeof(TAs), contract);
        return resolver;
    }

    /// <summary>
    /// Registers a factory for the given <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="TAs">The type to register as.</typeparam>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="factory">A factory method for generating a object of the specified type.</param>
    /// <param name="contract">A optional contract value which will indicates to only generate the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterAnd<TAs, T>(this IRegister resolver, Func<T> factory, string? contract = null)
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));
        factory.ThrowArgumentNullExceptionIfNull(nameof(factory));

        resolver.Register(() => factory()!, typeof(TAs), contract);
        return resolver;
    }

    /// <summary>
    /// Registers a constant value which will always return the specified object instance.
    /// </summary>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="value">The specified instance to always return.</param>
    /// <param name="serviceType">The type of service to register.</param>
    /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterConstantAnd(this IRegister resolver, object value, Type serviceType, string? contract = null)
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        resolver.Register(() => value, serviceType, contract);
        return resolver;
    }

    /// <summary>
    /// Registers a constant value which will always return the specified object instance.
    /// </summary>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterConstantAnd<T>(this IRegister resolver, string? contract = null)
        where T : new()
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        var value = new T();
        return resolver.RegisterAnd(() => value, contract);
    }

    /// <summary>
    /// Registers a constant value which will always return the specified object instance.
    /// </summary>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="value">The specified instance to always return.</param>
    /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterConstantAnd<T>(this IRegister resolver, T value, string? contract = null)
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        return resolver.RegisterAnd(() => value, contract);
    }

    /// <summary>
    /// Registers a lazy singleton value which will always return the specified object instance once created.
    /// The value is only generated once someone requests the service from the resolver.
    /// </summary>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="valueFactory">A factory method for generating a object of the specified type.</param>
    /// <param name="serviceType">The type of service to register.</param>
    /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterLazySingletonAnd(this IRegister resolver, Func<object> valueFactory, Type serviceType, string? contract = null)
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        var val = new Lazy<object>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        resolver.Register(() => val.Value, serviceType, contract);
        return resolver;
    }

    /// <summary>
    /// Registers a lazy singleton value which will always return the specified object instance once created.
    /// The value is only generated once someone requests the service from the resolver.
    /// </summary>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterLazySingletonAnd<T>(this IRegister resolver, string? contract = null)
        where T : new()
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        var val = new Lazy<object>(() => new T(), LazyThreadSafetyMode.ExecutionAndPublication);
        resolver.Register(() => val.Value, typeof(T), contract);
        return resolver;
    }

    /// <summary>
    /// Registers a lazy singleton value which will always return the specified object instance once created.
    /// The value is only generated once someone requests the service from the resolver.
    /// </summary>
    /// <typeparam name="T">The service type to register for.</typeparam>
    /// <param name="resolver">The resolver to register the service type with.</param>
    /// <param name="valueFactory">A factory method for generating a object of the specified type.</param>
    /// <param name="contract">A optional contract value which will indicates to only return the value if this contract is specified.</param>
    /// <returns>The resolver.</returns>
    public static IRegister RegisterLazySingletonAnd<T>(this IRegister resolver, Func<T> valueFactory, string? contract = null)
    {
        resolver.ThrowArgumentNullExceptionIfNull(nameof(resolver));

        var val = new Lazy<object>(() => valueFactory()!, LazyThreadSafetyMode.ExecutionAndPublication);
        resolver.Register(() => val.Value, typeof(T), contract);
        return resolver;
    }
}
