using System.Collections;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
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

        public override void OnNext(object value)
        {
            if (value is GuidValue { Guid: var guid, Value: ChildrenRequest { Data: var data, Filters: var filters } request })
            {

                FilterProperties(data, request.Guid, filters)
                .Subscribe(a =>
                {
                    Context.Post(_ =>
                    {
                        var (propertyNode, i) = a;
                        Broadcast(new GuidValue(guid, propertyNode, i));
                    },default);
                },
                e =>
                {
                    Broadcast(new GuidValue(guid, e, 0));
                },
                () =>
                {
                    Context.Post(_ =>
                    {
                        Broadcast(new GuidValue(guid, nameof(OnCompleted), 0));
                    }, default);
                });
            }
            else
            {
                base.OnNext(value);
            }


            IObservable<(PropertyNode, int i)> FilterProperties(object data, Guid guid, DescriptorFilters? filters = null)
            {
                Subject<(PropertyNode, int i)> subject = new();

                Task.Run(async () =>
                {
                    try
                    {
                        foreach (var prop in EnumerateProperties(data, guid, filters))
                        {
                            if (prop.Item1 != null)
                                subject.OnNext((await prop.Item1, prop.Item2));
                            else
                            {

                            }
                        }
                        subject.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        subject.OnError(ex);
                    }
                });

                return subject;


                IEnumerable<(Task<PropertyNode?>, int remaining)> EnumerateProperties(object data, Guid guid, DescriptorFilters? filters = null)
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
                        if (descriptor.PropertyType == typeof(Type))
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
        }
    }
}