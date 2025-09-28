using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;

namespace Utility.Nodify.Transitions.Demo
{
    public class NodeFactory : IFactory<IViewModelTree>
    {
        public IViewModelTree Create(object config)
        {
            var node = new Utility.Nodes.NodeViewModel() { Data = config, Input = [], Output = [] };
            //if (config is ISetNode iNode)
            //{
            //    iNode.SetNode(node);
            //}
            return node;
        }
    }
}
