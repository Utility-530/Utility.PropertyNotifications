using System.ComponentModel;
using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;

namespace Utility.PropertyTrees
{
    public static class PropertyDescriptorHelper
    {
        public static bool IsValueOrStringProperty(this PropertyDescriptor descriptor)
        {
            return IsValueOrStringProperty(descriptor.PropertyType);
        }
        public static bool IsValueOrStringProperty(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        public static PropertyType GetPropertyType(this PropertyDescriptor descriptor)
        {
            if (descriptor is CollectionItemDescriptor itemDescriptor)
                return PropertyType.CollectionItem | GetPropertyType(descriptor.PropertyType);
            else
                return GetPropertyType(descriptor.PropertyType);
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
