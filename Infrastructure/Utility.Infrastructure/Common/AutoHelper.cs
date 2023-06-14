using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Utility.Conversions;

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
        public static bool SetProperty<T>(this AutoObject autoObject, T value, [CallerMemberName] string name = null)
        {
            autoObject.SetProperty(value, typeof(T), name);
            return true;
        }

        /// <summary>
        /// Sets a property value.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>true if the value has changed; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool SetProperty(this AutoObject autoObject, object value, [CallerMemberName] string name = null)
        {
            autoObject.SetProperty(value, value.GetType(), name);
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
            return (T)(autoObject.GetProperty(typeof(T), name));
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
                return ConversionHelper.ChangeType(att.Value, defaultValue);
            }

            return defaultValue;
        }
    }
}