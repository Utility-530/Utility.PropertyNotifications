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

namespace Utility.PropertyTrees.Services
{
    public partial class ChildPropertyExplorer : BaseObject
    {
        public override Key Key => new(Guids.PropertyFilter, nameof(ChildPropertyExplorer), typeof(ChildPropertyExplorer));

        public Interfaces.Generic.IObservable<ChildrenResponse> OnNext(ChildrenRequest value)
        {
            var descriptors = PropertyDescriptors(value.Data).ToArray();
            var collectionDescriptors = CollectionItemDescriptors(value.Data).ToArray();
            var count = descriptors.Length + collectionDescriptors.Length;
            int i = 0;

            return Create<ChildrenResponse>(observer =>
            {
                CompositeDisposable composite = new();

                if (count == 0)
                    observer.OnCompleted();

                foreach (var descriptor in collectionDescriptors)
                {
                    FromCollectionDescriptor(descriptor)
                    .Subscribe(p =>
                    {
                        i++;
                        observer.OnNext(new ChildrenResponse(p, i, count));
                        if (count == i)
                            observer.OnCompleted();
                    })
                    .DisposeWith(composite);
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

            Interfaces.Generic.IObservable<ValueNode> FromCollectionDescriptor(CollectionItemDescriptor descriptor)
            {
                return Observe<ActivationResponse, ActivationRequest>(new(value.Guid, descriptor, descriptor.Item, GetType()))
                       .Select(a => a.PropertyNode);

                PropertyType GetType()
                {
                    if (IsValueOrStringProperty(descriptor))
                    {
                        return PropertyType.CollectionItem | PropertyType.Value;
                    }
                    else
                    {
                        return PropertyType.CollectionItem | PropertyType.Reference;
                    }
                }
            }


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
                }
            }

            static bool IsValueOrStringProperty(PropertyDescriptor? descriptor)
            {
                return descriptor.PropertyType.IsValueType || descriptor.PropertyType == typeof(string);
            }

            IEnumerable<PropertyDescriptor> PropertyDescriptors(object data) =>
                TypeDescriptor.GetProperties(data)
                    .Cast<PropertyDescriptor>()
                    .Where(a => value.Filters?.Any(f => f.Invoke(a) == false) == false)
                    .OrderBy(d => d.Name);

            IEnumerable<CollectionItemDescriptor> CollectionItemDescriptors(object data)
            {
                int i = 0;
                if (data is IEnumerable enumerable && data is not string s)
                    foreach (var item in enumerable)
                    {
                        var descriptor = new CollectionItemDescriptor(item, i);
                        if (value.Filters?.Any(f => f.Invoke(descriptor) == false) == false)
                            yield return descriptor;
                        i++;
                    }
                else
                    yield break;
            }
        }
    }
}
