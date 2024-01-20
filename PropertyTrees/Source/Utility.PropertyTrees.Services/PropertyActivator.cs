using Utility.Models;
using Utility.Infrastructure;
using System.Reactive.Linq;
using System.Collections.Concurrent;
using Utility.Nodes;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.NonGeneric;
using Utility.Observables.Generic;
using System.Reactive.Disposables;
using System.ComponentModel;
using Utility.PropertyDescriptors;

namespace Utility.PropertyTrees.Services
{
    public class PropertyActivator : BaseObject
    {
        private readonly ConcurrentDictionary<Key, PropertyBase> guids = new();

        public override Key Key => new(Guids.PropertyActivator, nameof(PropertyActivator), typeof(PropertyActivator));

        public override object? Model => guids;

        public Interfaces.Generic.IObservable<ActivationResponse> OnNext(ActivationRequest value)
        {
            var descriptor = value.Descriptor;
            var propertyType = value.PropertyType;
            var data = value.Data;

            return Create<ActivationResponse>(observer =>
            {
                return ToProperty(value.Key ?? Guid.NewGuid())
                .Subscribe(a =>
                {
                    observer.OnNext(new ActivationResponse(a));
                    observer.OnCompleted();
                });
            });

            Interfaces.Generic.IObservable<PropertyBase> ToProperty(Guid guid)
            {
                return propertyType switch
                {
                    PropertyType.Reference => CreateReferenceProperty(guid, descriptor, data),
                    PropertyType.Value => CreateValueProperty(guid, descriptor, data),
                    PropertyType.CollectionItem | PropertyType.Reference => CreateCollectionItemReferenceProperty(guid, descriptor, data),
                    PropertyType.CollectionItem | PropertyType.Value => CreateCollectionItemValueProperty(guid, descriptor, data),
                    PropertyType.Root => CreateRootProperty(guid, descriptor, data),
                    _ => throw new Exception("f 33 dsf"),
                };

                Interfaces.Generic.IObservable<PropertyBase> CreateReferenceProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    var item = descriptor.GetValue(data);
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }

                    return Create<PropertyBase>(observer2 =>
                    {
                        return Create<object>(observer =>
                        {
                            if (item == null)
                            {
                                if (descriptor.PropertyType.IsInterface)
                                {
                                    return Observe<TypeResponse, TypeRequest>(new(descriptor.PropertyType))
                                    .Subscribe(type =>
                                    {
                                        observer.OnNext(Activator.CreateInstance(type.Type));
                                        observer.OnCompleted();
                                    });
                                }
                                else
                                {
                                    observer.OnNext(Activator.CreateInstance(descriptor.PropertyType));
                                    observer.OnCompleted();
                                }
                            }
                            else
                            {
                                observer.OnNext(item);
                                observer.OnCompleted();
                            }
                            return Disposer<PropertyBase>.Empty;
                        }).Subscribe(item =>
                        {
                            descriptor.SetValue(data, item);
                            CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(ReferenceProperty))
                            .Subscribe(property =>
                            {
                                property.Descriptor = descriptor;
                                property.Data = item;
                                observer2.OnNext(property);
                            }, () => observer2.OnCompleted());

                        });
                    });
                }

                Interfaces.Generic.IObservable<PropertyBase> CreateValueProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }
                    return Create<PropertyBase>(observer =>
                    {
                        return CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(ValueProperty))

                        .Subscribe(property =>
                        {
                            property.Descriptor = descriptor;
                            property.Data = data;
                            observer.OnNext(property);
                        },
                        () => observer.OnCompleted());
                    });
                }

                Interfaces.Generic.IObservable<PropertyBase> CreateCollectionItemReferenceProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }

                    return Create<PropertyBase>(observer =>
                    {
                        return CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(CollectionItemReferenceProperty))
                        .Subscribe(property =>
                        {
                            property.Descriptor = descriptor;
                            property.Data = data;

                            observer.OnNext(property);
                        },
                        () => observer.OnCompleted());
                    });
                }

                Interfaces.Generic.IObservable<PropertyBase> CreateCollectionItemValueProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }

                    return Create<PropertyBase>(observer =>
                    {
                        return CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(CollectionItemValueProperty))
                        .Subscribe(property =>
                        {
                            property.Descriptor = descriptor;
                            property.Data = data;

                            observer.OnNext(property);
                        },
                        () => observer.OnCompleted());
                    });
                }

                Interfaces.Generic.IObservable<PropertyBase> CreateRootProperty(Guid parent, PropertyDescriptor descriptor, object data)
                {
                    if (descriptor == null)
                    {
                        throw new ArgumentNullException("descriptor");
                    }
                    if (data == null)
                    {
                        throw new Exception("f 33 gfg");
                    }
                    return Create<PropertyBase>(observer =>
                    {
                        return CreateInstance(parent, descriptor.Name, descriptor.PropertyType, typeof(RootProperty))
                        .Subscribe(property =>
                        {
                            property.Data = data;
                            observer.OnNext(property);
                        },
                        () => observer.OnCompleted());
                    });
                }

                Interfaces.Generic.IObservable<PropertyBase> CreateInstance(Guid parent, string name, Type propertyType, Type type)
                {
                    if (type == null)
                    {
                        throw new ArgumentNullException("type");
                    }
                    return Create<PropertyBase>(observer =>
                    {
                        CompositeDisposable composite = new();
                        Observe<FindPropertyResponse, FindPropertyRequest>(new(new Key(parent, name, propertyType)))
                        .Subscribe(result =>
                        {
                            Key key = result.Keys.SingleOrDefault() as Key ?? throw new Exception("dfb 43 4df");
                            //if (guids.ContainsKey(key))
                            //      observer.OnNext(guids[key]);
                            var args = new object[] { key.Guid };
                            Observe<ObjectCreationResponse, ObjectCreationRequest>(new(type, new[] { typeof(ValueNode), typeof(BaseObject) }, args))
                            .Subscribe(response =>
                            {
                                var instance = response.Instance as PropertyBase ?? throw new Exception("fg e4  ll;");
                                guids[key] = instance;
                                observer.OnNext(instance);
                                observer.OnCompleted();
                            },
                            () => observer.OnCompleted()).DisposeWith(composite);
                        }).DisposeWith(composite);
                        return composite;
                    });
                }
            }
        }
    }
}