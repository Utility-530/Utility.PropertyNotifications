using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Infrastructure;
using Utility.Models;
using ObservableExtensions = Utility.Observables.NonGeneric.ObservableExtensions;
using Utility.Observables.NonGeneric;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewModelEngine : BaseObject //, IPropertyGridEngine
    {
        public override Key Key => new(Guids.ViewModelEngine, nameof(ViewModelEngine), typeof(ViewModelEngine));

        public IObservable OnNext(IGuid data)
        {
            return ObservableExtensions.Create(observer =>
            {
                return Observe<ActivationResponse, ActivationRequest>(new(data.Guid, new RootDescriptor(data), data, PropertyType.Root))
                     .Select(a => a.PropertyNode)
                     .Subscribe(propertyNode =>
                     {
                         propertyNode.Data = data;
                         propertyNode.Predicates = new ViewModelPredicate();
                         observer.OnNext(propertyNode);
                         observer.OnCompleted();
                     });
            });
        }

        public class ViewModelPredicate : Filters
        {
            private List<Predicate<PropertyDescriptor>> predicates;

            public ViewModelPredicate()
            {
                predicates = new(){
                new Predicate<PropertyDescriptor>(descriptor=>
            {
                   return descriptor.PropertyType==typeof(IViewModel);
            }) };
            }

            public override IEnumerator<Predicate<PropertyDescriptor>> GetEnumerator()
            {
                return predicates.GetEnumerator();
            }
        }
    }
}