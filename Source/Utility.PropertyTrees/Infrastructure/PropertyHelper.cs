using System;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using Utility.Helpers.NonGeneric;

namespace Utility.PropertyTrees.Infrastructure
{
    public partial class PropertyFilter
    {
        public IEnumerable<(Task<PropertyNode?>, int remaining)> EnumerateProperties(object data, Guid guid, DescriptorFilters? filters = null)
        {
            var descriptors = PropertyDescriptors(data).ToArray();
            var count = descriptors.Length;
            int i = 0;

            if (data is IEnumerable enumerable && filters == null)
            {
                count += enumerable.Count();
                foreach (var item in enumerable)
                {
                    i++;
                    yield return (FromIndex(i, item), enumerable.Count() - i);
                }
            }

            foreach (var descriptor in descriptors)
            {
                i++;
                yield return (FromPropertyDescriptor(descriptor), count - i);
            }


            Task<PropertyNode?> FromIndex(int i, object? item)
            {
                return Observe<PropertyNode?, ActivationRequest>(new(guid, new CollectionItemDescriptor(item, i), item, PropertyType.CollectionItem)).ToTask();
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

            Task<PropertyNode?> FromPropertyDescriptor(PropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(MethodBase))
                    return null;
                if (descriptor.PropertyType == typeof(Model))
                    return null;

                return CreateProperty(data, guid, descriptor);

                async Task<PropertyNode?> CreateProperty(object data, Guid guid, PropertyDescriptor descriptor)
                {
                    PropertyNode property;
                    if (IsValueOrStringProperty(descriptor))
                    {
                        property = await Observe<PropertyNode, ActivationRequest>(new(guid, descriptor, data, PropertyType.Value)).ToTask();
                    }
                    else
                    {
                        property = await Observe<PropertyNode, ActivationRequest>(new(guid, descriptor, data, PropertyType.Reference)).ToTask();
                    }

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

    public class CollectionItemDescriptor : PropertyDescriptor
    {
        public CollectionItemDescriptor(object item, int index) : base(index.ToString(), null)
        {
            Item = item;
            Index = index;
        }

        public object Item { get; }

        public int Index { get; }

        public override Type ComponentType => throw new NotImplementedException();

        public override bool IsReadOnly => throw new NotImplementedException();

        public override Type PropertyType => Item.GetType();


        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            throw new NotImplementedException();
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }

    public class RootDescriptor : PropertyDescriptor
    {
        public RootDescriptor(object item) : base("root", null)
        {
            Item = item;
        }

        public object Item { get; }

        public override Type ComponentType => throw new NotImplementedException();

        public override bool IsReadOnly => throw new NotImplementedException();

        public override Type PropertyType => Item.GetType();


        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            throw new NotImplementedException();
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }
}


