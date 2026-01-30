using Utility.Entities;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Structs;

namespace Utility.Nodes.Demo.Lists.Services
{
    public class StringFilter : IFilter
    {
        public bool Filter(object o)
        {
            if (o is StringFilterParameters fq)
                return filter(fq);
            else if (o is INodeViewModel)
                return true;
            throw new Exception("44333 3gw");
        }

        private static bool filter(StringFilterParameters filterQuery)
        {
            switch (filterQuery)
            {
                case { Filter: { } filter, Value: UserProfile { Name: var name, UserName: var userName, Class: var @class, Group: var group } value }:
                    if (string.IsNullOrEmpty(filter))
                        return true;
                    return
                        name?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                        userName?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                        @class?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                        group?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true;
                //case { Filter: { } _filter, Value: SubscriptionModel { Name: var _name } }:
                //    if (string.IsNullOrEmpty(_filter))
                //        return true;
                //    return _name?.Contains(_filter, StringComparison.CurrentCultureIgnoreCase) == true;
                case { Filter: { } __filter, Value: AuctionItem { RelativePath: var title } }:
                    if (string.IsNullOrEmpty(__filter))
                        return true;
                    return title?.Contains(__filter, StringComparison.CurrentCultureIgnoreCase) == true;

                case { Filter: { } __filter, Value: Loan { Name: var title } }:
                    if (string.IsNullOrEmpty(__filter))
                        return true;
                    return title?.Contains(__filter, StringComparison.CurrentCultureIgnoreCase) == true;

                case { Filter: { } __filter, Value: Transaction { Description: var title } }:
                    if (string.IsNullOrEmpty(__filter))
                        return true;
                    return title?.Contains(__filter, StringComparison.CurrentCultureIgnoreCase) == true;

                case { Filter: { } __filter, Value: API.Services.Coinbase.CoinbaseTransaction { Notes: var title } }:
                    if (string.IsNullOrEmpty(__filter))
                        return true;
                    return title?.Contains(__filter, StringComparison.CurrentCultureIgnoreCase) == true;
                case { Filter: { } __filter, Value: Asset { Name: var title } }:
                    if (string.IsNullOrEmpty(__filter))
                        return true;
                    return title?.Contains(__filter, StringComparison.CurrentCultureIgnoreCase) == true;
                default:
                    throw new Exception("44333 3gw");
            }
        }
    }
}