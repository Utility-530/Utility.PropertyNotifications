using System.Collections;
using System.ComponentModel;
using System.Reactive.Subjects;
using Utility.Infrastructure;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{
    public abstract class DescriptorFilters : IEnumerable<Predicate<PropertyDescriptor>>
    {
        public abstract IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

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
                    var (propertyNode, i) = a;
                    Broadcast(new GuidValue(guid, propertyNode, i));
                },
                e =>
                {
                    Broadcast(new GuidValue(guid, e, 0));
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
                    }
                    catch (Exception ex)
                    {
                        subject.OnError(ex);
                    }
                });

                return subject;
            }
        }
    }
}