using System.ComponentModel;
using Utility.Models;
using Utility.Infrastructure;
using System.Reactive.Linq;

namespace Utility.PropertyTrees.Infrastructure
{
    public class PropertyActivator : BaseObject
    {
        Guid guid = Guid.Parse("24fadcc6-ac25-41a7-b482-fdf1a58b0ecd");
        public override Key Key => new Key(guid, nameof(PropertyActivator), typeof(PropertyActivator));
      
        public override async void OnNext(object value)
        {
            if (value is GuidValue { Value: ActivationRequest { Guid: var guid, Data: var data, Descriptor: var descriptor, PropertyType: var propertyType } } guidValue)
            {
                var newValue = new GuidValue(guidValue.Guid, await ToProperty(), 0);
                Broadcast(newValue);
            }
            else
            {
                base.OnNext(value);
            }

            Task<PropertyBase> ToProperty()
            {
                return propertyType switch
                {
                    PropertyType.Reference => CreateReferenceProperty(guid, descriptor, data),
                    PropertyType.Value => CreateValueProperty(guid, descriptor, data),
                    PropertyType.CollectionItem => CreateCollectionItemProperty(guid, descriptor, data),
                    PropertyType.Root => CreateRootProperty(guid, descriptor, data),
                    _ => throw new Exception("f 33 dsf"),
                };

                async Task<PropertyBase> CreateReferenceProperty(Guid parent, PropertyDescriptor descriptor, object data)
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
                            item = Activator.CreateInstance(await Observe<Type, Type>(descriptor.PropertyType));
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

                async Task<PropertyBase> CreateValueProperty(Guid parent, PropertyDescriptor descriptor, object data)
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

                async Task<PropertyBase> CreateCollectionItemProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }
                    var property = (CollectionItemProperty)await CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(CollectionItemProperty));

                    property.Descriptor = descriptor;
                    property.Data = data;

                    return property;
                }

                async Task<PropertyBase> CreateRootProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }
                    var property = (RootProperty)await CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(RootProperty));

                    property.Data = data;

                    return property;
                }

                async Task<PropertyBase> CreateInstance(Guid parent, string name, Type propertyType, Type type)
                {
                    if (type == null)
                    {
                        throw new ArgumentNullException("type");
                    }

                    var result = await Observe<FindResult, FindOrder>(new(new Key(parent, name, propertyType)));
                    Guid guid = (result.Key as Key)?.Guid ?? throw new Exception("dfb 43 4df");
                    var args = new object[] { guid };
                    var response = await Observe<ObjectCreationResponse, ObjectCreationRequest>(new(type, typeof(PropertyNode), args));
                    return response.Instance as PropertyBase ?? throw new Exception("fg e4  ll;");
                }
            }
        }
    }

    public record ObjectCreationRequest(Type Type, Type RegistrationType, object[] Args);
    public record ObjectCreationResponse(object Instance);

}