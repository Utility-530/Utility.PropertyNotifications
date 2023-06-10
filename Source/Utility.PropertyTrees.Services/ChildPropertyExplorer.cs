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
        public override Key Key => new(Guids.PropertyFilter, nameof(ChildPropertyExplorer), typeof(ChildPropertyExplorer));

        public Interfaces.Generic.IObservable<ChildrenResponse> OnNext(ChildrenRequest value)
        {
            var descriptors = PropertyDescriptors(value.Data).ToArray();


            var count = descriptors.Length + Count(value.Data); //+ collectionDescriptors.Length;
            int i = 0;

            return Create<ChildrenResponse>(observer =>
            {
                CompositeDisposable composite = new();

                Create<ChildrenResponse>(obs =>
                {
                    CompositeDisposable comp = new();
                    if (descriptors.Length == 0)
                        obs.OnCompleted();
                    foreach (var descriptor in descriptors)
                    {
                        Observe<DescriptorFilterResponse, DescriptorFilterRequest>(new DescriptorFilterRequest(descriptor))
                        .Subscribe(a =>
                        {
                            FromPropertyDescriptor(descriptor)
                            .Subscribe(p =>
                            {
                                i++;
                                if (a.Include)
                                    obs.OnNext(new ChildrenResponse(p, i, count));
                                observer.OnProgress(i, count);
                                if (descriptors.Length == i)
                                    obs.OnCompleted();
                            }).DisposeWith(comp);
                        }).DisposeWith(comp);
                    }

                    return comp;
                })
                .Subscribe(a => observer.OnNext(a),
                    e => observer.OnError(e),
                    () =>
                    {
                        var collectionDescriptors =
                        CollectionItemDescriptors(value.Data)
                        .Subscribe(descriptor =>
                        {
                            FromCollectionDescriptor(descriptor)
                            .Subscribe(p =>
                            {
                                i++;
                                observer.OnNext(new ChildrenResponse(p, i, count));
                                observer.OnProgress(i, count);
                                if (count == i && value.Data is not INotifyCollectionChanged)
                                    observer.OnCompleted();
                            })
                            .DisposeWith(composite);
                        },
                        () =>
                        {
                            observer.OnCompleted();
                        });
                    },

                (a, b) => observer.OnProgress(a, b));

                return composite;
            });


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


            IObservable<ValueNode?> FromPropertyDescriptor(PropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(MethodBase))
                    return Observable.Empty<ValueNode?>();
                if (descriptor.PropertyType == typeof(Type))
                    return Observable.Empty<ValueNode?>();

                return CreateProperty(value.Data, value.Guid, descriptor);

                IObservable<ValueNode> CreateProperty(object data, Guid guid, PropertyDescriptor descriptor)
                {
                    return Observable.Create<ValueNode>(observer =>
                    {
                        return Observe<ActivationResponse, ActivationRequest>(new(guid, descriptor, data, descriptor.GetPropertyType()))
                            .Subscribe(a => { observer.OnNext(a.PropertyNode); observer.OnCompleted(); });

                    });
                }
            }

            IEnumerable<PropertyDescriptor> PropertyDescriptors(object data) =>
                TypeDescriptor.GetProperties(data)
                    .Cast<PropertyDescriptor>()
                    //.Where(a => value.Filters?.Any(f => f.Invoke(a) == false) == false)
                    .OrderBy(d => d.Name);

            Utility.Interfaces.Generic.IObservable<CollectionItemDescriptor> CollectionItemDescriptors(object data)
            {
                return Create<CollectionItemDescriptor>(observer =>
                {
                    int i = 0;
                    if (data is IEnumerable enumerable && data is not string s)
                        foreach (var item in enumerable)
                        {
                            Next(value, observer, item, ref i);
                        }
                    if (data is INotifyCollectionChanged collectionChanged)
                    {
                        return collectionChanged.SelectNewItems<object>()
                        .Subscribe(item =>
                        {
                            Next(value, observer, item, ref i);
                        });                           
                    }
                    else
                    {
                        observer.OnCompleted();
                    }
                    return Disposer.Empty;
                });
                static void Next(ChildrenRequest value, Interfaces.Generic.IObserver<CollectionItemDescriptor> observer, object item, ref int i)
                {
                    var descriptor = new CollectionItemDescriptor(item, i);
                    //if (value.Filters?.Any(f => f.Invoke(descriptor) == false) == false)
                    observer.OnNext(descriptor);
                    i++;
                }
            }
        }
    }
}
