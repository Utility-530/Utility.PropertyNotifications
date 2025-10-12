using System.Reactive.Linq;
using Utility.Attributes;
using Utility.Entities;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Generic;
using Utility.Reactives;

namespace Utility.Nodes.Demo.Lists
{
    public class ModelTypesFactory : IEnumerableFactory<ModelType>
    {
        public IEnumerable<ModelType> Create(object? o = null)
        {
            return typeof(ModelTypesFactory)
                .Assembly
                .TypesByAttribute<ModelAttribute>(a => a.Index)
                .Select(type => new ModelType(type.Name, type));
        }
    }


}
