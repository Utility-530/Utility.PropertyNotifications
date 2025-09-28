using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodify.Base
{
    public record ConnectorParameters(Guid Guid, bool IsInput, string Key, INodeViewModel? Node, object Data);

    public record InstanceKey(string Key, object Data);

}
