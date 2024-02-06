using System.ComponentModel;

namespace Utility.PropertyDescriptors
{
    public static class PropertyDescriptorHelper
    {
        public static bool IsValueOrStringProperty(this System.ComponentModel.PropertyDescriptor descriptor)
        {
            return descriptor.PropertyType?.IsValueOrStringProperty()==true;
        }

        public static bool IsValueOrStringProperty(this Type type)
        {
            return type?.IsValueType == true || type == typeof(string);
        }

        public static PropertyType GetPropertyType(this System.ComponentModel.PropertyDescriptor descriptor)
        {
            if (descriptor is ICollectionItemDescriptor itemDescriptor)
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
