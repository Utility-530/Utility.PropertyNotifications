using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Nodify.Core;

namespace Utility.Nodify.Base
{
    public record NodeMessage(Key Key, IOValue[] Inputs, INodeViewModel Node, Exception Exception = default) : Message(Key, default)
    {

    }
}
