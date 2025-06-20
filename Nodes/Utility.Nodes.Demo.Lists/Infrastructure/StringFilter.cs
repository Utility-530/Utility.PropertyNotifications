using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Entities;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Entities;

namespace Utility.Nodes.Demo.Lists.Infrastructure
{
    public class StringFilter : IFilter
    {
        public bool Filter(object o)
        {
            if (o is FilterQuery { Filter: { } filter, Value: UserProfileModel { Name: var name, UserName: var userName, Class: var @class, Group: var group } value })
            {
                if (string.IsNullOrEmpty(filter))
                    return true;
                return name?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                    userName?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                    @class?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true ||
                    group?.Contains(filter, StringComparison.CurrentCultureIgnoreCase) == true;
            }
            else if (o is INode)
                return true;
            throw new Exception("44333 3gw");
        }
    }
}
