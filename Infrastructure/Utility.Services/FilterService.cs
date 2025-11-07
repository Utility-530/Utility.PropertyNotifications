using Splat;
using Utility.Interfaces.NonGeneric;
using Utility.Services.Meta;
using Utility.Structs;

namespace Utility.Services
{
    public record FilterParam() : Param<FilterService>(nameof(FilterService.Filter), "filter");
    public record PredicateReturnParam() : Param<FilterService>(nameof(FilterService.Filter));

    public class FilterService
    {
        public static Predicate<object> Filter(string filter) => new((v) => Locator.Current.GetService<IFilter>().Filter(new StringFilterParameters(filter, v)));
    }
}