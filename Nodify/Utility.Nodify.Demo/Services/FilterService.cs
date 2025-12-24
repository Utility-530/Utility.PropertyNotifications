using System;
using Splat;
using Utility.Entities;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Nodify.Demo.Services
{
    public record FilterParam() : Param<FilterService>(nameof(FilterService.Filter), "filter");
    public record PredicateReturnParam() : Param<FilterService>(nameof(FilterService.Filter));


    public class FilterService
    {
        //public static Predicate<object> Filter(string filter) => new((v) => Locator.Current.GetService<IFilter>().Filter(new FilterQuery(filter, v)));
        public static Predicate<object> Filter(string filter) =>  throw new Exception("sdfdsfsddsdfsd");
    }
}
