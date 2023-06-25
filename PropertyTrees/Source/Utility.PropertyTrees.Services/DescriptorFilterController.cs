using System.ComponentModel;
using Utility.Infrastructure;
using Utility.Models;
using Utility.PropertyTrees.Infrastructure;
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



        public IObservable<MethodInfoFilterResponse> OnNext(MethodInfoFilterRequest filterRequest)
        {
            return Create((Func<IObserver<MethodInfoFilterResponse>, IDisposable>)(observer =>
            {
                observer.OnNext((new MethodInfoFilterResponse(filterRequest.MethodInfo, IsFiltered(filterRequest))));
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
                if (descriptor.IsBrowsable == false)
                    return false;

                if (descriptor.IsReadOnly && descriptor.PropertyType.IsCollection() == false)
                    return false;

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


        private static string[] names = new[] { "Send" , "Connect", "Foo", "Bar" };

        private static bool IsFiltered(MethodInfoFilterRequest filterRequest)
        {
            var value = filterRequest.MethodInfo;
          

            if (value.ReflectedType.Name != "Events" && value.Name == "Add")
                return true;
            if (value.ReflectedType.Name != "Events" && value.Name == "Clear")
                return true;
            if (value.ReflectedType.Name != "Events" && value.Name == "Remove")
                return true;            
            if (value.ReflectedType.Name != "Events" && value.Name == "MoveUp")
                return true;    
            if (value.ReflectedType.Name != "Events" && value.Name == "MoveDown")
                return true;  
            if (names.Contains(value.Name))
                return true;
            return false;
        }
    }

    public static class PropertyTypeHelper
    {
        public static bool IsCollection(this Type propertyType) => propertyType != null && propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType);
        public static bool IsException(this PropertyDescriptor descriptor) => typeof(Exception).IsAssignableFrom(descriptor.PropertyType);
    }

}
