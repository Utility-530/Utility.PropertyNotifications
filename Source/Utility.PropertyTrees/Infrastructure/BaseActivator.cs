using Utility.PropertyTrees;
using Utility.PropertyTrees.Abstractions;
using System.ComponentModel;

namespace Utility.PropertyTrees.Infrastructure
{
    public class BaseActivator
    {
        public static Dictionary<Type, Type> Interfaces { get; set; }

        public static PropertyStore PropertyStore { get; set; }

        public virtual async Task<object?> CreateInstance(Guid parent, string name, Type propertyType, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var cx = await PropertyStore.GetGuidByParent(new Key(parent, name, propertyType));

            var args = new object[]
            {
               cx
            };
            return Activator.CreateInstance(type, args);
        }

        public async Task<IProperty> CreateReferenceProperty(Guid parent, PropertyDescriptor descriptor, object data)
        {
            var item = descriptor.GetValue(data);
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }
            if (item == null)
            {
                if (descriptor.PropertyType.IsInterface)
                {
                    item = Activator.CreateInstance(Interfaces[descriptor.PropertyType]);
                }
                else
                {
                    item = Activator.CreateInstance(descriptor.PropertyType);
                }
                descriptor.SetValue(data, item);
            }
            var property = (ReferenceProperty)await CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(ReferenceProperty));

            property.Descriptor = descriptor;
            property.Data = item;

            return property;
        }

        public async Task<IProperty> CreateValueProperty(Guid parent, PropertyDescriptor descriptor, object data)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }
            if (data == null)
            {
                throw new Exception("f 33 gfg");
            }
            var property = (ValueProperty)await CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(ValueProperty));

            property.Descriptor = descriptor;
            property.Data = data;

            return property;
        }

        public async Task<IProperty> CreateCollectionItemProperty(Guid parent, int index, object data)
        {
            var property = (CollectionItemProperty)await CreateInstance(parent, index.ToString(), data.GetType(), typeof(CollectionItemProperty));

            property.Index = index;
            property.Data = data;

            return property;
        }

        public static BaseActivator Instance { get; } = new();
    }
}