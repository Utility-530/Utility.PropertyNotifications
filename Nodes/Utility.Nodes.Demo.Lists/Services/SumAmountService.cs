using System.Collections;
using Utility.Attributes;
using Utility.Entities;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Lists.Services
{
    [ParamAttribute(Enums.CLREvent.CollectionChanged)]
    public record SumAmountInputParam() : Param<SumAmountService>(nameof(SumAmountService.Sum), "list", true);

    public record SumAmountReturnParam() : Param<SumAmountService>(nameof(SumAmountService.Sum));

    public class SumAmountService
    {
        public static string Sum(IList list) => list.OfType<IGetAmount>().Aggregate(0m, (a, b) => a + b.Amount).ToString();
    }
}