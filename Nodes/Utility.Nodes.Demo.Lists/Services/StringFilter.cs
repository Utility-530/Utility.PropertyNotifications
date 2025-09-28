using Utility.Entities;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Entities;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Models;

namespace Utility.Nodes.Demo.Lists.Services
{
    public class StringFilter : IFilter
    {
        public bool Filter(object o)
        {
            if (o is FilterQuery fq)
                return filter(fq);
            else if (o is INodeViewModel)
                return true;
            throw new Exception("44333 3gw");
        }

        private static bool filter(FilterQuery filterQuery) 
        {
            switch (filterQuery)
            {
                case { Filter: { } filter, Value: UserProfileModel { Name: var name, UserName: var userName, Class: var @class, Group: var group } value }:
                    if (string.IsNullOrEmpty(filter))
                        return true;
                    return
                        name?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                        userName?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                        @class?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                        group?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true;
                case { Filter: { } _filter, Value: SubscriptionModel { Name: var _name } }:
                    if (string.IsNullOrEmpty(_filter))
                        return true;
                    return _name?.Contains(_filter, StringComparison.CurrentCultureIgnoreCase) == true;
                case { Filter: { } __filter, Value: EbayModel { RelativePath: var title } }:
                    if (string.IsNullOrEmpty(__filter))
                        return true;
                    return title?.Contains(__filter, StringComparison.CurrentCultureIgnoreCase) == true;
                default:
                    throw new Exception("44333 3gw");
            }
        }
    }
}
