using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;

namespace Utility.Nodify.Demo
{
    public class NodeFactory : IFactory<INode>
    {
        public INode Create(object config)
        {
            var node = new Utility.Nodes.Node() { Data = config };
            if (config is ISetNode iNode)
            {
                iNode.SetNode(node);
            }
            return node;
        }
    }
}
