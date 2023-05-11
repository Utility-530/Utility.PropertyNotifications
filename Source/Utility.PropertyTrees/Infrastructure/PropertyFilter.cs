using System.Collections;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Helpers.NonGeneric;
using Utility.Infrastructure;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{

    public partial class PropertyFilter : BaseObject
    {
        public Guid Guid => Guid.Parse("215cdcc6-ac25-41a7-b482-fdf1a58b0ecd");

        public override Key Key => new(Guid, nameof(PropertyFilter), typeof(PropertyFilter));

        public override bool OnNext(object value)
        {
            if (value is GuidValue { Guid: var guid, Value: ChildrenRequest { Data: var data, Filters: var filters } request })
            {

                FilterProperties(data, request.Guid, filters)
                .Subscribe(a =>
                {
                    var (propertyNode, i) = a;
                    Broadcast(new GuidValue(guid, propertyNode, i));
                },
                e =>
                {
                    Broadcast(new GuidValue(guid, e, 0));
                },
                () =>
                {
                    Broadcast(new GuidValue(guid, nameof(OnCompleted), 0));
                });
                return true;

            }
            else
            {
                return base.OnNext(value);
            }

            IObservable<(PropertyNode?, int)> FilterProperties(object data, Guid guid, DescriptorFilters? filters = null)
            {
                var descriptors = PropertyDescriptors(data).ToArray();
                var count = descriptors.Length;
                int i = 0;

                return Observable.Create<(PropertyNode?, int)>(observer =>
                {
                    CompositeDisposable composite = new();
                    if (data is IEnumerable enumerable && filters == null)
                    {
                        count += enumerable.Count();
                        foreach (var item in enumerable)
                        {
                            i++;
                            count++;
                            FromIndex(i, item)
                            .Subscribe(a =>
                            {
                                observer.OnNext((a, count));
                                if (--count == 0)
                                    observer.OnCompleted();
                            })
                            .DisposeWith(composite);
                        }
                    }

                    foreach (var descriptor in descriptors)
                    {
                        FromPropertyDescriptor(descriptor)
                        .Subscribe(a =>
                        {
                            observer.OnNext((a, count));
                            if (--count == 0)
                                observer.OnCompleted();
                        })
                        .DisposeWith(composite);
                    }
                    return composite;
                });

                IObservable<PropertyNode?> FromIndex(int i, object? item)
                {
                    return Observe<PropertyNode?, ActivationRequest>(new(guid, new CollectionItemDescriptor(item, i), item, PropertyType.CollectionItem));
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

                IObservable<PropertyNode?> FromPropertyDescriptor(PropertyDescriptor descriptor)
                {
                    if (descriptor.PropertyType == typeof(MethodBase))
                        return Observable.Return<PropertyNode?>(default);
                    if (descriptor.PropertyType == typeof(Type))
                        return Observable.Return<PropertyNode?>(default);

                    return CreateProperty(data, guid, descriptor);

                    IObservable<PropertyNode> CreateProperty(object data, Guid guid, PropertyDescriptor descriptor)
                    {
                        return Observable.Create<PropertyNode>(observer =>
                        {
                            if (IsValueOrStringProperty(descriptor))
                            {
                                return Observe<PropertyNode, ActivationRequest>(new(guid, descriptor, data, PropertyType.Value))
                                        .Subscribe(a => { observer.OnNext(a); observer.OnCompleted(); });
                            }
                            else
                            {
                                return Observe<PropertyNode, ActivationRequest>(new(guid, descriptor, data, PropertyType.Reference))
                                         .Subscribe(a => { observer.OnNext(a); observer.OnCompleted(); });

                            }
                        });

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
}