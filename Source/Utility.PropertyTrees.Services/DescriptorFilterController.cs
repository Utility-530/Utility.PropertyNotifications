using System.ComponentModel;
using Utility.Infrastructure;
using Utility.Models;
using Utility.PropertyTrees.Infrastructure;
using static Utility.PropertyTrees.Events;
using static System.Reactive.Linq.Observable;
using System.Collections;
using Utility.Observables.NonGeneric;

namespace Utility.PropertyTrees.Services
{
    public class DescriptorFilterController : BaseObject
    {
        public override Key Key => new Key<DescriptorFilterController>(Guids.DescriptorFilter);     

        public IObservable<DescriptorFilterResponse> OnNext(DescriptorFilterRequest filterRequest)
        {
            return Create<DescriptorFilterResponse>(observer =>
            {
                var value = filterRequest.PropertyDescriptor;
                if (value is CollectionItemDescriptor collectionItemDescriptor)
                     observer.OnNext(new DescriptorFilterResponse(value, true));
                if (value is PropertyDescriptor descriptor)
                {
                    if (descriptor.IsException())
                    {
                        observer.OnNext(new DescriptorFilterResponse(value, false));
                    }
                    if (descriptor.ComponentType.IsCollection())
                    {
                        observer.OnNext(new DescriptorFilterResponse(value, false));
                    }
                    //int level = descriptor.PropertyType.InheritanceLevel();
                    if(descriptor.IsBrowsable==false)
                    {
                        observer.OnNext(new DescriptorFilterResponse(value, false));
                    }                  
                }

                observer.OnNext(new DescriptorFilterResponse(value, true));
                return Disposer.Empty;
            });
        }
    }

    public static class PropertyTypeHelper
    {
        public static bool IsCollection(this Type propertyType) => propertyType != null && propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType);
        public static bool IsException(this PropertyDescriptor descriptor) => typeof(Exception).IsAssignableFrom(descriptor.PropertyType);
    }

}
