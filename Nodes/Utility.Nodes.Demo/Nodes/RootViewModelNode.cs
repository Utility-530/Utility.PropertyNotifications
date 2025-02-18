using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodes.Demo.Infrastructure;

namespace Utility.Nodes.Demo
{
    public class RootViewModelNode : ViewModelNode
    {
        public RootViewModelNode() : base(Locator.Current.GetService<Resolver>().RootType)
        {

        }           

    }


}
