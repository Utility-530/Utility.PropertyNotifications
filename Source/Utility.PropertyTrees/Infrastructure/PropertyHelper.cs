using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Utility.PropertyTrees.Infrastructure
{
    public static class PropertyHelper
    {
        private static PropertyActivator activator = PropertyActivator.Instance;

        public static IEnumerable<PropertyNode> EnumerateProperties(object data, Guid guid, DescriptorFilters? filters = null)
        {

            if (data is IEnumerable enumerable && filters == null)
            {
                int i = 0;
                foreach (var item in enumerable)
                {
                    yield return FromIndex(i, item);
                    i++;
                }
            }

            var descriptors = PropertyDescriptors(data).ToArray();
            foreach (var descriptor in descriptors)
            {
                yield return FromPropertyDescriptor(descriptor);
            }


            PropertyNode? FromIndex(int i, object? item)
            {
                return activator.CreateCollectionItemProperty(guid, i, item).Result;
            }

            IEnumerable<PropertyDescriptor> PropertyDescriptors(object data)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(data)
                    .Cast<PropertyDescriptor>()
                    .Where(a => filters?.All(f => f.Invoke(a)) != false)
                    .OrderBy(d => d.Name))
                {
                    yield return descriptor;
                }
            }

            PropertyNode? FromPropertyDescriptor(PropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(MethodBase))
                    return null;
                if (descriptor.PropertyType == typeof(Model))
                    return null;

                return CreateProperty(data, guid, descriptor);

                PropertyNode CreateProperty(object data, Guid guid, PropertyDescriptor descriptor)
                {
                    PropertyNode property;
                    if (IsValueOrStringProperty(descriptor))
                    {
                        property = activator.CreateValueProperty(guid, descriptor, data).Result;
                    }
                    else/* if(IsCollectionProperty(descriptor))*/
                    {
                        property = activator.CreateReferenceProperty(guid, descriptor, data).Result;
                    }
                    //else
                    //{
                    //    var item = descriptor.GetValue(data);
                    //    property = activator.CreateProperty(guid, descriptor, item).Result;
                    //}

                    return property;

                    static bool IsValueOrStringProperty(PropertyDescriptor? descriptor)
                    {
                        return descriptor.PropertyType.IsValueType || descriptor.PropertyType == typeof(string);
                    }

                    static bool IsCollectionProperty(PropertyDescriptor? descriptor)
                    {
                        return descriptor.PropertyType != null ? descriptor.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(descriptor.PropertyType) : false;
                    }
                }
            }
        }

    }
}


