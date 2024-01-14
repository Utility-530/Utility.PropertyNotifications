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
        public RootViewModelNode() : base(Resolver.Instance.Root)
        {

        }
    }
}
