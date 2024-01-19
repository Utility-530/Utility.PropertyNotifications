using System.ComponentModel;
using Utility.PropertyDescriptors;

namespace Utility.Properties
{
    public static class PropertyDescriptorHelper
    {
        public static bool IsValueOrStringProperty(this PropertyDescriptor descriptor)
        {
            return descriptor.PropertyType.IsValueOrStringProperty();
        }
        public static bool IsValueOrStringProperty(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        public static PropertyType GetPropertyType(this PropertyDescriptor descriptor)
        {
            if (descriptor is CollectionItemDescriptor itemDescriptor)
                return PropertyType.CollectionItem | descriptor.PropertyType.GetPropertyType();
            else
                return descriptor.PropertyType.GetPropertyType();
        }

        public static PropertyType GetPropertyType(this Type type)
        {

            if (type.IsValueOrStringProperty())
            {
                return PropertyType.Value;
            }
            else
            {
                return PropertyType.Reference;
            }

        }
    }
}
