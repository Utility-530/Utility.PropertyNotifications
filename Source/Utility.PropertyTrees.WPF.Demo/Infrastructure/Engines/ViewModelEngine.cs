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

namespace Utility.PropertyTrees.WPF.Demo
{
    public class ViewModelEngine : BaseObject, IPropertyGridEngine
    {
        Guid guid = Guid.Parse("78f35bd1-fc3c-44ca-8d86-f3a8a9d69d33");

        public ViewModelEngine()
        {
        }

        public override Key Key => new(guid, nameof(ViewModelEngine), typeof(ViewModelEngine));

        public async Task<IPropertyNode> Convert(object data)
        {
            if (data is IGuid guid)
            {
                var propertyNode = await Observe<PropertyNode, ActivationRequest>(new(guid.Guid, new RootDescriptor(data), data, PropertyType.Root)).ToTask();
                propertyNode.Data = data;
                propertyNode.Predicates = new ViewModelPredicate();
                return propertyNode;
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