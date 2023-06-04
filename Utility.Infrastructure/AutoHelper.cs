using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Utility.Conversions;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{
    public static class AutoHelper
    {
        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool SetProperty<T>(this AutoObject autoObject, T value, [CallerMemberName] string? name = null)
        {
            autoObject.SetProperty(new Key(autoObject.Guid, name, typeof(T)), value);
            return true;
        }

        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The value automatically converted into the requested type.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T GetProperty<T>(this AutoObject autoObject, [CallerMemberName] string? name = null)
        {
            return (T)autoObject.GetProperty(new Key(autoObject.Guid, name, typeof(T)));
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool SetProperty<T>(this T autoObject, object value)
            where T : AutoObject, IType
        {
            autoObject.SetProperty(autoObject.Key, value);
            return true;
        }


        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The value automatically converted into the requested type.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object GetProperty<T>(this T autoObject)
            where T : AutoObject, IType
        {
            return autoObject.GetProperty(autoObject.Key);
        }
        
        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool SetProperty<T, TValue>(this T autoObject, TValue value, [CallerMemberName] string? name = null)
            where T : AutoObject, IType
        {
            autoObject.SetProperty((new Key(autoObject.Guid, name, typeof(T))), value);
            return true;
        }


        /// <summary>
        /// Gets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <returns>The value automatically converted into the requested type.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TValue GetProperty<T, TValue>(this T autoObject, [CallerMemberName] string? name = null)
            where T : AutoObject, IType
        {
            return (TValue)autoObject.GetProperty(new Key(autoObject.Guid, name, typeof(TValue)));
        }

        /// <summary>
        /// Gets the default value for a given property.
        /// </summary>
        /// <param name="propertyName">The property name. May not be null.</param>
        /// <returns>The default value. May be null.</returns>
        public static object GetDefaultValue(PropertyDescriptor descriptor)
        {
            if (descriptor.Name == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            PropertyInfo propertyInfo = descriptor.ComponentType.GetProperty(descriptor.Name);
            if (propertyInfo == null)
            {
                //if (ThrowOnInvalidProperty)
                //{
                //throw new InvalidOperationException(SR.GetString("invalidPropertyName", GetType().FullName, propertyName));
                throw new InvalidOperationException("invalidPropertyName");
                // }

                //  return null;
            }

            object defaultValue = propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(propertyInfo.PropertyType) : null;
            DefaultValueAttribute att = Attribute.GetCustomAttribute(propertyInfo, typeof(DefaultValueAttribute), true) as DefaultValueAttribute;
            if (att != null)
            {
                return ConversionHelper.ChangeType(att.Value, defaultValue: defaultValue);
            }

            return defaultValue;
        }
    }
}