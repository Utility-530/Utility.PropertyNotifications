using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Utility.Interfaces.NonGeneric;
using Utility.Infrastructure;
using Utility.Models;
using System.Threading.Tasks;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewModelEngine : BaseObject, IPropertyGridEngine
    {
        Guid guid = Guid.Parse("78f35bd1-fc3c-44ca-8d86-f3a8a9d69d33");

        public ViewModelEngine()
        {
        }

        public override Key Key => new(guid, nameof(ViewModelEngine), typeof(ViewModelEngine));

        public Task<IPropertyNode> Convert(object data) => Observe(data).ToTask();

        public IObservable<IPropertyNode> Observe(object data)
        {
            if (data is IGuid guid)
            {
                return Observable.Create<PropertyNode>(observer =>
                {
                    return Observe<PropertyNode, ActivationRequest>(new(guid.Guid, new RootDescriptor(data), data, PropertyType.Root))
                         .Subscribe(propertyNode =>
                         {
                             propertyNode.Data = data;
                             propertyNode.Predicates = new ViewModelPredicate();
                             observer.OnNext(propertyNode);
                             observer.OnCompleted();
                         });
                });
            }
            throw new Exception(" 4 wewfwe");
        }

        public class ViewModelPredicate : DescriptorFilters
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