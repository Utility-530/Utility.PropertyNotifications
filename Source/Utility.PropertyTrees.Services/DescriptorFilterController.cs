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
            return Create((Func<IObserver<DescriptorFilterResponse>, IDisposable>)(observer =>
            {
                observer.OnNext((new DescriptorFilterResponse(filterRequest.PropertyDescriptor, IsFiltered(filterRequest))));
                return Disposer.Empty;
            }));
        }

        private static bool IsFiltered(DescriptorFilterRequest filterRequest)
        {
            var value = filterRequest.PropertyDescriptor;
            if (value is CollectionItemDescriptor collectionItemDescriptor)
            {
                return true;
            }
            if (value is PropertyDescriptor descriptor)
            {
                if (descriptor.IsException())
                {
                    return false;
                }
                if (descriptor.ComponentType.IsCollection())
                {
                    return false;
                }
                //int level = descriptor.PropertyType.InheritanceLevel();
                if (descriptor.IsBrowsable == false)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public static class PropertyTypeHelper
    {
        public static bool IsCollection(this Type propertyType) => propertyType != null && propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType);
        public static bool IsException(this PropertyDescriptor descriptor) => typeof(Exception).IsAssignableFrom(descriptor.PropertyType);
    }

}
