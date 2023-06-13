using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Utility.Common;

/// <summary>
/// AutoMoqer mocks all constructor dependencies for a service
/// </summary>
/// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
public class AutoMoqer
{
    private readonly ConstructorInfo _primaryConstructor;
    private readonly Dictionary<Type, object> _exceptionParametersByType = new Dictionary<Type, object>();
    private readonly Dictionary<string, object> _exceptionParametersByName = new Dictionary<string, object>();
    private readonly Type type;

    public AutoMoqer(Type type)
    {
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        if (constructors.Length > 1)
            throw new ArgumentException("Multiple public constructors found");

        var primaryConstructor = constructors.FirstOrDefault();
        if (primaryConstructor == null)
            throw new ArgumentException("Could not find a public constructor");

        _primaryConstructor = primaryConstructor;
        this.type = type;
    }

    /// <summary>
    /// Use instance for type TParam instead of creating a Mock-object for that paramter
    /// </summary>
    /// <typeparam name="TParam">The type of parameter to replace</typeparam>
    /// <param name="instance">The value to use instead of a Mock-object</param>
    /// <returns>An instance to the current AutoMoqer object</returns>
    public AutoMoqer With<TParam>(object instance)
    {
        if (_exceptionParametersByType.ContainsKey(typeof(TParam)))
            throw new ArgumentException($"An instance for the parameter with type {typeof(TParam).Name} has already been registered");

        _exceptionParametersByType.Add(typeof(TParam), instance);
        return this;
    }

    /// <summary>
    /// Use instance for parameter named name instead of creating a Mock-object
    /// </summary>
    /// <param name="name">The type of parameter to replace</param>
    /// <param name="instance">The value to use instead of a Mock-object</param>
    /// <returns>An instance to the current AutoMoqer object</returns>
    public AutoMoqer With(string name, object instance)
    {
        var nameLowerCase = name.ToLower();

        if (_exceptionParametersByName.ContainsKey(nameLowerCase))
            throw new ArgumentException($"An instance for the parameter named {name} has already been registered");

        _exceptionParametersByName.Add(nameLowerCase, instance);
        return this;
    }

    /// <summary>
    /// Create a new <see cref="AutoMoqer{TService}"/> container with the current configuration.
    /// </summary>
    /// <returns>A new <see cref="AutoMoqer{TService}"/> container</returns>
    public AutoMoqerContainer Build()
    {
        //Clone the lists to support the creation of multiple independent containers from the same AutoMoqer-object
        var exceptionParametersByTypeCopy = new Dictionary<Type, object>(_exceptionParametersByType);
        var exceptionParametersByNameCopy = new Dictionary<string, object>(_exceptionParametersByName);

        return new AutoMoqerContainer(
            type,
            _primaryConstructor,
            exceptionParametersByTypeCopy,
            exceptionParametersByNameCopy);
    }

    /// <summary>
    /// Create a new <see cref="AutoMoqerContainerWithExplicitVerification{TService}"/> container with the current configuration.
    /// </summary>
    /// <returns>A new <see cref="AutoMoqerContainerWithExplicitVerification{TService}"/> container</returns>
    public AutoMoqerContainerWithExplicitVerification BuildWithExplicitVerification()
    {
        //Clone the lists to support the creation of multiple independent containers from the same AutoMoqer-object
        var exceptionParametersByTypeCopy = new Dictionary<Type, object>(_exceptionParametersByType);
        var exceptionParametersByNameCopy = new Dictionary<string, object>(_exceptionParametersByName);

        return new AutoMoqerContainerWithExplicitVerification(
            type,
            _primaryConstructor,
            exceptionParametersByTypeCopy,
            exceptionParametersByNameCopy);
    }

    ///// <summary>
    ///// A container for AutoMoqer that holds all constructor dependencies for a service
    ///// </summary>
    ///// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    //public class AutoMoqerContainer<TService> : AutoMoqerContainerBase<TService>, IDisposable
    //{
    //    internal AutoMoqerContainer(
    //        ConstructorInfo primaryConstructor,
    //        IReadOnlyDictionary<Type, object> exceptionParametersByType,
    //        IReadOnlyDictionary<string, object> exceptionParametersByName)
    //        : base(primaryConstructor, exceptionParametersByType, exceptionParametersByName)
    //    {
    //        // nothing to do here
    //    }

    //    /// <summary>
    //    /// Will run VerifyAll on all Moq-parameters
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        var exceptionOccurred = Marshal.GetExceptionPointers() != IntPtr.Zero || Marshal.GetExceptionCode() != 0;
    //        if (exceptionOccurred)
    //            return;

    //        VerifyAllInstances();
    //    }
    //}

    /// <summary>
    /// A container for AutoMoqer that holds all constructor dependencies for a service
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainer : AutoMoqerContainerBase, IDisposable
    {
        internal AutoMoqerContainer(
            Type type,
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
            : base(type, primaryConstructor, exceptionParametersByType, exceptionParametersByName)
        {
            // nothing to do here
        }

        /// <summary>
        /// Will run VerifyAll on all Moq-parameters
        /// </summary>
        public void Dispose()
        {
            var exceptionOccurred = Marshal.GetExceptionPointers() != IntPtr.Zero || Marshal.GetExceptionCode() != 0;
            if (exceptionOccurred)
                return;

            //VerifyAllInstances();
        }
    }

    /// <summary>
    /// Represents the base class of the container for AutoMoqer that holds all constructor dependencies for a service and allows verifying all expectations.
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainerBase<TService>
    {
        //private readonly List<object> _moqInstancesParameters = new List<object>();
        private readonly Lazy<TService> _serviceInstance;

        internal AutoMoqerContainerBase(
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
        {
            var serviceConstructionParameters = new List<object>();

            var parameters = primaryConstructor.GetParameters();
            foreach (var parameter in parameters)
            {
                var exceptionByType = exceptionParametersByType.ContainsKey(parameter.ParameterType);
                var exceptionByName = exceptionParametersByName.ContainsKey(parameter.Name.ToLower());
                if (exceptionByType && exceptionByName)
                    throw new ArgumentException($"Parameter named {parameter.Name} has multiple registered exceptions (by type and/or by name)");

                if (exceptionByName)
                {
                    serviceConstructionParameters.Add(exceptionParametersByName.First(p => p.Key == parameter.Name.ToLower()).Value);
                }
                else if (exceptionByType)
                {
                    serviceConstructionParameters.Add(exceptionParametersByType.First(p => p.Key == parameter.ParameterType).Value);
                }
                else
                {
                    object? parameterInstance;
                    object? parameterMockInstance;
                    if (parameter.ParameterType.IsValueType == false)
                    {
                        //Create and add Moq-instance for parameter
                        var genericType = typeof(Mock<>);
                        var genericGenericType = genericType.MakeGenericType(parameter.ParameterType);
                        parameterInstance = Activator.CreateInstance(genericGenericType);
                        parameterMockInstance = parameterInstance.GetType().GetProperty("Object", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).GetValue(parameterInstance, null);
                    }
                    else
                    {
                        parameterInstance = Activator.CreateInstance(parameter.ParameterType);
                        parameterMockInstance = parameterInstance;
                    }
                    //  _moqInstancesParameters.Add(parameterInstance);
                    serviceConstructionParameters.Add(parameterMockInstance);
                }
            }

            //Create new service with mock parameter and provided parameters
            _serviceInstance = new Lazy<TService>(() => (TService)Activator.CreateInstance(typeof(TService), serviceConstructionParameters.ToArray()));
        }

        /// <summary>
        /// Get constructor parameter Mock instance
        /// </summary>
        /// <typeparam name="TParam">Type of constructor parameter</typeparam>
        /// <returns>A mock instance of type TParam, or null of no such parameter could be resolved</returns>
        //public Mock<TParam> Param<TParam>() where TParam : class
        //{
        //    var genericType = typeof(Mock<>);
        //    var genericGenericType = genericType.MakeGenericType(typeof(TParam));

        //    var param = _moqInstancesParameters.SingleOrDefault(p => p.GetType() == genericGenericType);
        //    if (param == null)
        //        throw new ArgumentException($"Parameter with type {typeof(TParam).Name} not found in constructor parameter mock list");

        //    return (Mock<TParam>)Convert.ChangeType(param, typeof(Mock<TParam>));
        //}

        /// <summary>
        /// Instance to the service instance
        /// </summary>
        public TService Service => _serviceInstance.Value;

        /// <summary>
        /// Creates the service.
        /// </summary>
        public void CreateService()
        {
            var unused = _serviceInstance.Value;
        }

        /// <summary>
        /// Will run VerifyAll on all Moq-parameters
        /// </summary>
        //protected void VerifyAllInstances()
        //{
        //    foreach (var parameter in _moqInstancesParameters)
        //    {
        //        var method = parameter.GetType().GetMethod("VerifyAll");
        //        method.Invoke(parameter, null);
        //    }
        //}
    }

    /// <summary>
    /// Represents the base class of the container for AutoMoqer that holds all constructor dependencies for a service and allows verifying all expectations.
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainerBase
    {
        // private readonly List<object> _moqInstancesParameters = new List<object>();
        private readonly Lazy<object> _serviceInstance;

        private readonly Type type;

        internal AutoMoqerContainerBase(
            Type type,
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
        {
            var serviceConstructionParameters = new List<object>();

            var parameters = primaryConstructor.GetParameters();
            foreach (var parameter in parameters)
            {
                var exceptionByType = exceptionParametersByType.ContainsKey(parameter.ParameterType);
                var exceptionByName = exceptionParametersByName.ContainsKey(parameter.Name.ToLower());
                if (exceptionByType && exceptionByName)
                    throw new ArgumentException($"Parameter named {parameter.Name} has multiple registered exceptions (by type and/or by name)");

                if (exceptionByName)
                {
                    serviceConstructionParameters.Add(exceptionParametersByName.First(p => p.Key == parameter.Name.ToLower()).Value);
                }
                else if (exceptionByType)
                {
                    serviceConstructionParameters.Add(exceptionParametersByType.First(p => p.Key == parameter.ParameterType).Value);
                }
                else
                {
                    if (parameter.ParameterType.IsValueType)
                        throw new ArgumentException($"Unable to create Moq-object for parameter named {parameter.Name} as Moq doesn't support value-types");

                    //Create and add Moq-instance for parameter

                    object parameterMockInstance = null;

                    if (parameter.ParameterType == typeof(string))
                    {
                        parameterMockInstance = string.Empty;
                    }
                    else if (parameter.ParameterType == typeof(Type))
                    {
                        parameterMockInstance = typeof(object);
                    }
                    else if (parameter.ParameterType.IsArray)
                    {
                        parameterMockInstance = Array.CreateInstance(parameter.ParameterType.GetElementType(), 0);
                    }
                    else if (parameter.ParameterType.IsAbstract)
                    {
                        parameterMockInstance = null;
                    }
                    //else if (parameter.ParameterType.ContainsGenericParameters)
                    //{
                    //    parameterMockInstance = null;
                    //}
                    else if (parameter.ParameterType.IsSealed)
                    {
                        parameterMockInstance = Activator.CreateInstance(parameter.ParameterType);
                    }
                    else
                    {
                        var genericType = typeof(Mock<>);
                        var genericGenericType = genericType.MakeGenericType(parameter.ParameterType);
                        var parameterInstance = Activator.CreateInstance(genericGenericType);
                        parameterMockInstance = parameterInstance.GetType().GetProperty("Object", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).GetValue(parameterInstance, null);
                    }

                    //  _moqInstancesParameters.Add(parameterMockInstance);
                    serviceConstructionParameters.Add(parameterMockInstance);
                }
            }

            //Create new service with mock parameter and provided parameters
            _serviceInstance = new Lazy<object>(() => Activator.CreateInstance(type, serviceConstructionParameters.ToArray()));
            this.type = type;
        }

        /// <summary>
        /// Get constructor parameter Mock instance
        /// </summary>
        /// <typeparam name="TParam">Type of constructor parameter</typeparam>
        /// <returns>A mock instance of type TParam, or null of no such parameter could be resolved</returns>
        //public Mock<TParam> Param<TParam>() where TParam : class
        //{
        //    var genericType = typeof(Mock<>);
        //    var genericGenericType = genericType.MakeGenericType(typeof(TParam));

        //    var param = _moqInstancesParameters.SingleOrDefault(p => p.GetType() == genericGenericType);
        //    if (param == null)
        //        throw new ArgumentException($"Parameter with type {typeof(TParam).Name} not found in constructor parameter mock list");

        //    return (Mock<TParam>)Convert.ChangeType(param, typeof(Mock<TParam>));
        //}

        /// <summary>
        /// Instance to the service instance
        /// </summary>
        public object Service => _serviceInstance.Value;

        /// <summary>
        /// Creates the service.
        /// </summary>
        public void CreateService()
        {
            var unused = _serviceInstance.Value;
        }

        /// <summary>
        /// Will run VerifyAll on all Moq-parameters
        /// </summary>
        //protected void VerifyAllInstances()
        //{
        //    foreach (var parameter in _moqInstancesParameters)
        //    {
        //        var method = parameter.GetType().GetMethod("VerifyAll");
        //        method.Invoke(parameter, null);
        //    }
        //}
    }

    /// <summary>
    /// A container for AutoMoqer that holds all constructor dependencies for a service and allows verifying all expectations.
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainerWithExplicitVerification<TService> : AutoMoqerContainerBase<TService>
    {
        protected internal AutoMoqerContainerWithExplicitVerification(
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
            : base(primaryConstructor, exceptionParametersByType, exceptionParametersByName)
        {
            // nothing to do here
        }

        /// <summary>
        /// Will run <see cref="Moq.Mock.VerifyAll"/> on all Moq-parameters.
        /// </summary>
        //public void VerifyAll()
        //{
        //    VerifyAllInstances();
        //}
    }

    /// <summary>
    /// A container for AutoMoqer that holds all constructor dependencies for a service and allows verifying all expectations.
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainerWithExplicitVerification : AutoMoqerContainerBase
    {
        protected internal AutoMoqerContainerWithExplicitVerification(
            Type type,
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
            : base(type, primaryConstructor, exceptionParametersByType, exceptionParametersByName)
        {
            // nothing to do here
        }

        /// <summary>
        /// Will run <see cref="Moq.Mock.VerifyAll"/> on all Moq-parameters.
        /// </summary>
        //public void VerifyAll()
        //{
        //    VerifyAllInstances();
        //}
    }
}
