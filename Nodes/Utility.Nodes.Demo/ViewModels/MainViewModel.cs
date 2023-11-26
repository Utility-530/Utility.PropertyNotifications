using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.ViewModels
{
    public class MainViewModel
    {
        public ITree Value { get; } = new DemoRootNode();

    }
}
