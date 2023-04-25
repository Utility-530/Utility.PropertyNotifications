using Utility.PropertyTrees.Abstractions;
using System.Collections;
using System.ComponentModel;
using System.Reactive.Subjects;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyTrees.Infrastructure
{
    //public interface IGuidConverter
    //{
    //    Guid Convert(object data);
    //}

    //public class GuidConverter : IGuidConverter
    //{
    //    public Guid Convert(object data)
    //    {
    //        if (data is IGuid iguid)
    //        {
    //            return iguid.Guid;
    //        }
    //        throw new Exception("esfdd 33");
    //    }
    //}

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
        public IObservable<IProperty> FilterProperties(object data, Guid guid, DescriptorFilters? filters = null)
        {
            Subject<IProperty> subject = new();

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