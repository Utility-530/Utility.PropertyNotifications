using System.Collections.Generic;
using Utility.Nodify.Core;

namespace Utility.Nodify.Operations.Infrastructure
{
    public interface INodeSource
    {
        IEnumerable<Node> FindAll();
        Node Find(string key);
    }
}
