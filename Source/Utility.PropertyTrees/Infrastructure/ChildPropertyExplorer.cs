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

namespace Utility.PropertyTrees.Infrastructure
{
    public partial class ChildPropertyExplorer : BaseObject
    {
        public override Key Key => new(Guids.PropertyFilter, nameof(ChildPropertyExplorer), typeof(ChildPropertyExplorer));

        public Utility.Interfaces.Generic.IObservable<ChildrenResponse> OnNext(ChildrenRequest value)
        {
            var descriptors = PropertyDescriptors(value.Data).ToArray();
            var count = descriptors.Length;
            int i = 0;

            return Create<ChildrenResponse>(observer =>
            {
                CompositeDisposable composite = new();
                if (value.Data is IEnumerable enumerable && value.Filters == null)
                {
                    count += enumerable.Count();
                    foreach (var item in enumerable)
                    {
                        FromIndex(i++, item)
                        .Subscribe(p =>
                        {
                            observer.OnNext(new ChildrenResponse(p, i, count));
                            if (count == i)
                                observer.OnCompleted();
                        })
                        .DisposeWith(composite);
                    }
                }

                foreach (var descriptor in descriptors)
                {
                    FromPropertyDescriptor(descriptor)
                    .Subscribe(p =>
                    {
                        i++;
                        observer.OnNext(new ChildrenResponse(p, i, count));
                        if (count == i)
                            observer.OnCompleted();
                    })
                    .DisposeWith(composite);
                }

                return composite;
            });

            Interfaces.Generic.IObservable<ValueNode> FromIndex(int i, object? item)
            {
                return Observe<ActivationResponse, ActivationRequest>(new(value.Guid, new CollectionItemDescriptor(item, i), item, PropertyType.CollectionItem))
                        .Select(a => a.PropertyNode);
            }

            IEnumerable<PropertyDescriptor> PropertyDescriptors(object data) => 
                TypeDescriptor.GetProperties(data)
                    .Cast<PropertyDescriptor>()
                    .Where(a => value.Filters?.All(f => f.Invoke(a)) != false)
                    .OrderBy(d => d.Name);

            IObservable<ValueNode?> FromPropertyDescriptor(PropertyDescriptor descriptor)
            {
                if (descriptor.PropertyType == typeof(MethodBase))
                    return Observable.Return<ValueNode?>(default);
                if (descriptor.PropertyType == typeof(Type))
                    return Observable.Return<ValueNode?>(default);

                return CreateProperty(value.Data, value.Guid, descriptor);

                IObservable<ValueNode> CreateProperty(object data, Guid guid, PropertyDescriptor descriptor)
                {
                    return Observable.Create<ValueNode>(observer =>
                    {
                        if (IsValueOrStringProperty(descriptor))
                        {
                            return Observe<ActivationResponse, ActivationRequest>(new(guid, descriptor, data, PropertyType.Value))
                                    .Subscribe(a => { observer.OnNext(a.PropertyNode); observer.OnCompleted(); });
                        }
                        else
                        {
                            return Observe<ActivationResponse, ActivationRequest>(new(guid, descriptor, data, PropertyType.Reference))
                                     .Subscribe(a => { observer.OnNext(a.PropertyNode); observer.OnCompleted(); });

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
