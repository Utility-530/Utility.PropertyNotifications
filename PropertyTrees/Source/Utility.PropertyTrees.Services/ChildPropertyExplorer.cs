using System.Collections;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Helpers.NonGeneric;
using Utility.Infrastructure;
using Utility.Models;
using Utility.Nodes;
using Utility.Observables.NonGeneric;
using static Utility.Observables.Generic.ObservableExtensions;
using Utility.Observables.Generic;
using Utility.PropertyTrees.Infrastructure;
using System.Collections.Specialized;
using Utility.Helpers.Ex;
using Utility.Helpers;
using Utility.Nodes.Abstractions;

namespace Utility.PropertyTrees.Services
{
    public partial class ChildPropertyExplorer : BaseObject
    {
        readonly Dictionary<Type, PropertyDescriptor[]> cachedPropertyDesciptors = new();
        readonly Dictionary<PropertyDescriptor, bool> includes = new();

        public override Key Key => new(Guids.PropertyFilter, nameof(ChildPropertyExplorer), typeof(ChildPropertyExplorer));

        public Interfaces.Generic.IObservable<ChildrenResponse> OnNext(ChildrenRequest value)
        {

            var descriptors = cachedPropertyDesciptors.GetValueOrCreate(value.Data.GetType(), () => PropertyDescriptors(value.Descriptor).ToArray());
            int count = descriptors.Length + Count(value.Data);
            int i = 0;
            CompositeDisposable composite = new();

            return Create<ChildrenResponse>(observer =>
            {
                Create<ChildrenResponse>(obs =>
                {
                    if (count == 0)
                    {
                        observer.OnNext(new ChildrenResponse(Change<INode>.None, Change<PropertyDescriptor>.None));
                    }
                    foreach (var descriptor in descriptors)
                    {
                        if (includes.TryGetValue(descriptor, out bool include) == false)
                            Observe<DescriptorFilterResponse, DescriptorFilterRequest>(new DescriptorFilterRequest(descriptor))
                            .Subscribe(a =>
                            {
                                SubscribeToPropertyDescriptor(descriptor, includes[descriptor] = a.Include, obs);
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
                        if (descriptor.Type == ChangeType.Add)
                            FromCollectionDescriptor(descriptor.Value)
                            .Subscribe(p =>
                            {
                                i++;
                                obs.OnNext(new ChildrenResponse(new Change<INode>(p, ChangeType.Add), descriptor.As<PropertyDescriptor>()));
                                obs.OnProgress(i, count);
                                if (count == i)
                                {
                                    obs.OnNext(new ChildrenResponse(new Change<INode>(default, ChangeType.None), default));
                                    if (value.Data is not INotifyCollectionChanged)
                                      obs.OnCompleted();
                                }
                            })
                            .DisposeWith(composite);
                        else if (descriptor.Type == ChangeType.Remove)
 
                                obs.OnNext(new ChildrenResponse(new Change<INode>(default, ChangeType.Remove), descriptor.As<PropertyDescriptor>()));
     
                        else if (descriptor.Type == ChangeType.Reset)
                            obs.OnNext(new ChildrenResponse(new Change<INode>(default, ChangeType.Reset), new Change<PropertyDescriptor>(default, ChangeType.Reset)));
                    },
                    () =>
                    {
                        //observer.OnCompleted();
                    });

                Interfaces.Generic.IObservable<ValueNode> FromCollectionDescriptor(CollectionItemDescriptor descriptor)
                {
                    return Observe<ActivationResponse, ActivationRequest>(new(value.Guid, descriptor, descriptor.Item, descriptor.GetPropertyType()))
                           .Select(a => a.PropertyNode);
                }
            }

            void SubscribeToPropertyDescriptor(PropertyDescriptor descriptor, bool include, Interfaces.Generic.IObserver<ChildrenResponse> obs)
            {
                if (include == false)
                {
                    i++;
                    obs.OnNext(new ChildrenResponse(Change<INode>.None, new Change<PropertyDescriptor>(descriptor, ChangeType.Add)));
                    obs.OnProgress(i, count);
                    if (descriptors.Length == i)
                    {
                        SubscribeToCollectionItemDescriptors(obs);
                    }
                    return;
                }

                NodeFromPropertyDescriptor(descriptor)
                    .Subscribe(p =>
                    {
                        i++;
                        obs.OnNext(new ChildrenResponse(new(p, ChangeType.Add), new(descriptor, ChangeType.Add)));
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

            IObservable<ValueNode?> NodeFromPropertyDescriptor(PropertyDescriptor descriptor)
            {
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

            IEnumerable<PropertyDescriptor> PropertyDescriptors(PropertyDescriptor data) =>
                data.GetChildProperties()
                    .Cast<PropertyDescriptor>()
                    .OrderBy(d => d.Name);

            Utility.Interfaces.Generic.IObservable<Change<CollectionItemDescriptor>> CollectionItemDescriptors(object data)
            {
                return Create<Change<CollectionItemDescriptor>>(observer =>
                {
                    int i = 0;
                    if (data is IEnumerable enumerable && data is not string s)
                        foreach (var item in enumerable)
                        {
                            Next(value, observer, item, data.GetType(), ChangeType.Add, ref i);
                        }
                    if (data is INotifyCollectionChanged collectionChanged)
                    {
                        collectionChanged.SelectChanges()
                        .Subscribe(a =>
                        {
                            switch (a.Action)
                            {
                                case NotifyCollectionChangedAction.Add:
                                    foreach (var item in a.NewItems)
                                        Next(value, observer, item, data.GetType(), ChangeType.Add, ref i);
                                    break;
                                case NotifyCollectionChangedAction.Remove:
                                    foreach (var item in a.OldItems)
                                    {
                                        var descriptor = new CollectionItemDescriptor(item, (data as IList).IndexOf(item), data.GetType());
                                        observer.OnNext(new(descriptor, ChangeType.Remove));
                                    }
                                    break;
                                case NotifyCollectionChangedAction.Reset:
                                    observer.OnNext(new(null, ChangeType.Reset));
                                    break;
                            }
                        });
                    }
                    else
                    {
                        //observer.OnCompleted();
                    }
                    return Disposer.Empty;
                });
                static void Next(ChildrenRequest value, Interfaces.Generic.IObserver<Change<CollectionItemDescriptor>> observer, object item, Type componentType, ChangeType changeType, ref int i)
                {
                    var descriptor = new CollectionItemDescriptor(item, i, componentType);
                    //if (value.Filters?.Any(f => f.Invoke(descriptor) == false) == false)
                    observer.OnNext(new(descriptor, changeType));
                    i++;
                }
            }
        }
    }
}
