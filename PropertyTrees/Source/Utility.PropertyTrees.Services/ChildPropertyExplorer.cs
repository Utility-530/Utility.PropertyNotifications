using System.Collections;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Helpers.NonGeneric;
using Utility.Infrastructure;
using Utility.Models;
using Utility.Nodes;
using Utility.Observables.NonGeneric;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.Generic;
using static Utility.PropertyTrees.Events;
using Utility.PropertyTrees.Infrastructure;
using System.Collections.Specialized;
using Utility.Helpers.Ex;

namespace Utility.PropertyTrees.Services
{
    public partial class ChildPropertyExplorer : BaseObject
    {
        readonly Dictionary<Type, PropertyDescriptor[]> propertyDesciptors = new();
        readonly Dictionary<PropertyDescriptor, bool> includes = new();
        public override Key Key => new(Guids.PropertyFilter, nameof(ChildPropertyExplorer), typeof(ChildPropertyExplorer));

        public Interfaces.Generic.IObservable<ChildrenResponse> OnNext(ChildrenRequest value)
        {
            if (propertyDesciptors.TryGetValue(value.Data.GetType(), out PropertyDescriptor[]? descriptors) == false)
                descriptors = propertyDesciptors[value.Data.GetType()] = PropertyDescriptors(value.Data).ToArray();

            int count = descriptors.Length + Count(value.Data);
            int i = 0;
            CompositeDisposable composite = new();

            return Create<ChildrenResponse>(observer =>
            {
                Create<ChildrenResponse>(obs =>
                {
                    //if (descriptors.Length == 0)
                    //    obs.OnCompleted();
                    if (count == 0)
                    {
                        observer.OnNext(new ChildrenResponse(null, false));
                    }
                    foreach (var descriptor in descriptors)
                    {
                        if (includes.TryGetValue(descriptor, out bool include) == false)
                            Observe<DescriptorFilterResponse, DescriptorFilterRequest>(new DescriptorFilterRequest(descriptor))
                            .Subscribe(a =>
                            {
                                SubscribeToPropertyDescriptor(descriptor, a.Include, obs);
                            })
                            .DisposeWith(composite);
                        else
                            SubscribeToPropertyDescriptor(descriptor, include, obs);
                    }

                    return composite;
                })
                .Subscribe(a => observer.OnNext(a),
                    e => observer.OnError(e),
                    () =>
                    {
                        //observer.OnCompleted();
                    },
                    (a, b) => observer.OnProgress(a, b));

                return composite;
            });

            void SubscribeToCollectionItemDescriptors(Interfaces.Generic.IObserver<ChildrenResponse> obs)
            {
                var collectionDescriptors =
                    CollectionItemDescriptors(value.Data)
                    .Subscribe(descriptor =>
                    {
                        FromCollectionDescriptor(descriptor)
                        .Subscribe(p =>
                        {
                            i++;
                            obs.OnNext(new ChildrenResponse(p, true));
                            obs.OnProgress(i, count);
                            if (count == i && value.Data is not INotifyCollectionChanged)
                                obs.OnCompleted();
                        })
                        .DisposeWith(composite);
                    },
                    () =>
                    {
                        //observer.OnCompleted();
                    });
            }

            void SubscribeToPropertyDescriptor(PropertyDescriptor descriptor, bool include, Interfaces.Generic.IObserver<ChildrenResponse> obs)
            {
                NodeFromPropertyDescriptor(descriptor)
                       .Subscribe(p =>
                       {
                           i++;
                           obs.OnNext(new ChildrenResponse(p, include));
                           obs.OnProgress(i, count);
                           if (descriptors.Length == i)
                           {
                               SubscribeToCollectionItemDescriptors(obs);
                           }
                       }).DisposeWith(composite);
            }

            int Count(object data)
            {
                if (data is IEnumerable enumerable)
                {
                    return enumerable.Count();
                }
                return 0;
            }

            Interfaces.Generic.IObservable<ValueNode> FromCollectionDescriptor(CollectionItemDescriptor descriptor)
            {
                return Observe<ActivationResponse, ActivationRequest>(new(value.Guid, descriptor, descriptor.Item, descriptor.GetPropertyType()))
                       .Select(a => a.PropertyNode);
            }


            IObservable<ValueNode?> NodeFromPropertyDescriptor(PropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(MethodBase))
                    return Observable.Empty<ValueNode?>();
                if (descriptor.PropertyType == typeof(Type))
                    return Observable.Empty<ValueNode?>();

                return Observable.Create<ValueNode>(observer =>
                    {
                        return Observe<ActivationResponse, ActivationRequest>(new(value.Guid, descriptor, value.Data, descriptor.GetPropertyType()))
                            .Subscribe(a =>
                            {
                                observer.OnNext(a.PropertyNode);
                                observer.OnCompleted();
                            });
                    });
            }

            IEnumerable<PropertyDescriptor> PropertyDescriptors(object data) =>
                TypeDescriptor.GetProperties(data)
                    .Cast<PropertyDescriptor>()
                    .OrderBy(d => d.Name);

            Utility.Interfaces.Generic.IObservable<CollectionItemDescriptor> CollectionItemDescriptors(object data)
            {
                return Create<CollectionItemDescriptor>(observer =>
                {
                    int i = 0;
                    if (data is IEnumerable enumerable && data is not string s)
                        foreach (var item in enumerable)
                        {
                            Next(value, observer, item, data.GetType(), ref i);
                        }
                    if (data is INotifyCollectionChanged collectionChanged)
                    {
                        return collectionChanged.SelectNewItems<object>()
                        .Subscribe(item =>
                        {
                            Next(value, observer, item, data.GetType(), ref i);
                        });
                    }
                    else
                    {
                        //observer.OnCompleted();
                    }
                    return Disposer.Empty;
                });
                static void Next(ChildrenRequest value, Interfaces.Generic.IObserver<CollectionItemDescriptor> observer, object item, Type componentType, ref int i)
                {
                    var descriptor = new CollectionItemDescriptor(item, i, componentType);
                    //if (value.Filters?.Any(f => f.Invoke(descriptor) == false) == false)
                    observer.OnNext(descriptor);
                    i++;
                }
            }
        }
    }
}
