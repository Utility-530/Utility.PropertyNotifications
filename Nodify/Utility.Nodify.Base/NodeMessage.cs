using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodify.Core;

namespace Utility.Nodify.Base
{
    public record NodeMessage(string Key, IOValue[] Inputs, INodeViewModel Node, Exception Exception = default) : Message(Key, default)
    {

    }
}
