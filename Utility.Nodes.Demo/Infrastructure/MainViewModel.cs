using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodes.Abstractions;

namespace Utility.Nodes.Demo.Infrastructure
{
    public class MainViewModel
    {
        public INode Value { get; } = new TypeNode();

    }
}
