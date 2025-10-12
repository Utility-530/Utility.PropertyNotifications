using System.Collections;
using Utility.Models;
using Utility.Nodes.Demo.Lists.Models;

namespace Utility.Nodes.Demo.Lists.Services
{

    public record SumCollectionInputParam() : MethodParameter<SumCollectionService>(nameof(SumCollectionService.Sum), "list");

    public record SumCollectionReturnParam() : MethodParameter<SumCollectionService>(nameof(SumCollectionService.Sum));

    public class SumCollectionService
    {
        public static string Sum(IList list) => list.OfType<CreditCardModel>().Aggregate(0m, (a, b) => a + b.Amount).ToString();
    }
}
