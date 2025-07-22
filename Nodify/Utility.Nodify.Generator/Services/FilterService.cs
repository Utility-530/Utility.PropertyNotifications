using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Entities;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Nodify.Generator.Services
{
    public record FilterParam() : MethodParameter<FilterService>(nameof(FilterService.Filter), "filter");
    public record PredicateReturnParam() : MethodParameter<FilterService>(nameof(FilterService.Filter));


    public class FilterService
    {
        public static Predicate<object> Filter(string filter) => new((v) => Locator.Current.GetService<IFilter>().Filter(new FilterQuery(filter, v)));
    }
}
