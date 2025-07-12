using Splat;
using Utility.Entities;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Services
{
    public record FilterParam() : MethodParameter<FilterService>(nameof(FilterService.Filter), "filter");
    public record PredicateReturnParam() : MethodParameter<FilterService>(nameof(FilterService.Filter));


    public class FilterService
    {
        public static Predicate<object> Filter(string filter)=> new((v) => Locator.Current.GetService<IFilter>().Filter(new FilterQuery(filter, v)));
    }
}
