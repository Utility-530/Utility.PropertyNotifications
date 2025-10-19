using Utility.API.Services;
using Utility.Entities;
using Utility.Interfaces.Generic;
using static Utility.API.Services.Coinbase;

namespace Utility.Nodes.Demo.Lists
{
    public class ModelTypesFactory : IEnumerableFactory<Type>
    {
        public IEnumerable<Type> Create(object? o = null)
        {
            yield return typeof(AuctionItem);
            yield return typeof(Loan);
            yield return typeof(UserProfile);
            yield return typeof(Transaction);
            yield return typeof(Asset);
            yield return typeof(CoinbaseTransaction);


            //return typeof(ModelTypesFactory)
            //    .Assembly
            //    .TypesByAttribute<ModelAttribute>(a => a.Index);
        }
    }


}
