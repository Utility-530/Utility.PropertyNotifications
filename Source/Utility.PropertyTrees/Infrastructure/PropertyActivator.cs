using System.ComponentModel;
using Utility.Models;
using Utility.Infrastructure;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using Utility.Observables;
using System.Reactive.Disposables;

namespace Utility.PropertyTrees.Infrastructure
{
    public class PropertyActivator : BaseObject
    {
        Guid guid = Guid.Parse("24fadcc6-ac25-41a7-b482-fdf1a58b0ecd");
        public override Key Key => new(guid, nameof(PropertyActivator), typeof(PropertyActivator));
        ConcurrentDictionary<Guid, PropertyBase> guids = new();

        public override bool OnNext(object value)
        {
            if (value is GuidValue { Value: ActivationRequest { Guid: var parentGuid, Data: var data, Descriptor: var descriptor, PropertyType: var propertyType } } guidValue)
            {
                _ =  ToProperty(parentGuid ?? Guid.NewGuid())
                    .Subscribe(property =>
                    {
                        Broadcast(new GuidValue(guidValue.Guid, property, 0));
                    });
                return true;
            }
            else
            {
                return base.OnNext(value);
            }

            IObservable<PropertyBase> ToProperty(Guid guid)
            {
                return propertyType switch
                {
                    PropertyType.Reference => CreateReferenceProperty(guid, descriptor, data),
                    PropertyType.Value => CreateValueProperty(guid, descriptor, data),
                    PropertyType.CollectionItem => CreateCollectionItemProperty(guid, descriptor, data),
                    PropertyType.Root => CreateRootProperty(guid, descriptor, data),
                    _ => throw new Exception("f 33 dsf"),
                };

                IObservable<PropertyBase> CreateReferenceProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    var item = descriptor.GetValue(data);
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }

                    return Observable.Create<PropertyBase>(observer2 =>
                    {
                        return Observable.Create<object>(observer =>
                        {
                            if (item == null)
                            {
                                if (descriptor.PropertyType.IsInterface)
                                {
                                    return Observe<Type, Type>(descriptor.PropertyType)
                                    .Subscribe(type =>
                                    {
                                        observer.OnNext(Activator.CreateInstance(type));
                                        observer.OnCompleted();
                                    });
                                }
                                else
                                {
                                    observer.OnNext(Activator.CreateInstance(descriptor.PropertyType));
                                    observer.OnCompleted();
                                } 
                          
                            }
                            return Disposable.Empty;
                        }).Subscribe(item =>
                        {
                            descriptor.SetValue(data, item);
                            CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(ReferenceProperty))
                            .Cast<ReferenceProperty>()
                            .Subscribe(property =>
                            {

                                property.Descriptor = descriptor;
                                property.Data = item;
                                observer2.OnNext(property);
                            },() => observer2.OnCompleted());
                   
                        });
                    });
                }

                IObservable<PropertyBase> CreateValueProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }
                    return Observable.Create<PropertyBase>(observer =>
                    {
                        return CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(ValueProperty))
                        .Cast<ValueProperty>()
                        .Subscribe(property =>
                        {
                            property.Descriptor = descriptor;
                            property.Data = data;
                            observer.OnNext(property);
                        }, 
                        () => observer.OnCompleted());
                    });
                }

                IObservable<PropertyBase> CreateCollectionItemProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }

                    return Observable.Create<PropertyBase>(observer =>
                    {
                        return CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(CollectionItemProperty))
                        .Cast<CollectionItemProperty>()
                        .Subscribe(property =>
                        {
                            property.Descriptor = descriptor;
                            property.Data = data;

                            observer.OnNext(property);
                        },
                        () => observer.OnCompleted());
                    });
                }

                IObservable<PropertyBase> CreateRootProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }
                    return Observable.Create<PropertyBase>(observer =>
                    {
                        return CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(RootProperty))
                        .Cast<RootProperty>()
                        .Subscribe(property =>
                        {
                            property.Data = data;
                            observer.OnNext(property);
                        },
                        () => observer.OnCompleted());
                    });
                }

                IObservable<PropertyBase> CreateInstance(Guid parent, string name, System.Type propertyType, System.Type type)
                {
                    if (type == null)
                    {
                        throw new ArgumentNullException("type");
                    }
                    return Observable.Create<PropertyBase>(observer =>
                    {
                        return Observe<FindResult, FindRequest>(new(new Key(parent, name, propertyType)))
                           .Subscribe(result =>
                           {
                               Guid guid = (result.Key as Key)?.Guid ?? throw new Exception("dfb 43 4df");
                               if (guids.ContainsKey(guid))
                                   observer.OnNext(guids[guid]);
                               var args = new object[] { guid };
                               Observe<ObjectCreationResponse, ObjectCreationRequest>(new(type, typeof(PropertyNode), args))
                               .Subscribe(response =>
                               {
                                   var instance = response.Instance as PropertyBase ?? throw new Exception("fg e4  ll;");
                                   guids[guid] = instance;
                                   observer.OnNext(instance);
                               },
                               () => observer.OnCompleted());
                           });
                    });
                }
            }
        }
    }

    public record ObjectCreationRequest(System.Type Type, System.Type RegistrationType, object[] Args);
    public record ObjectCreationResponse(object Instance);

}