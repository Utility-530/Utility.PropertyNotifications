using Utility.PropertyTrees.Abstractions;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Subjects;

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

    internal class PropertyFilter
    {
        public IObservable<PropertyNode> FilterProperties(object data, Guid guid, DescriptorFilters? filters = null)
        {
            Subject<PropertyNode> subject = new();

            Task.Run(() =>
            {
                try
                {
                    foreach (var prop in PropertyHelper.EnumerateProperties(data, guid, filters))
                    {
                        if (prop != null)
                            subject.OnNext(prop);
                    }
                }
                catch(Exception ex)
                {
                    subject.OnError(ex);
                }
            });

            return subject;
        }


        public static PropertyFilter Instance { get; } = new PropertyFilter();
    }
}