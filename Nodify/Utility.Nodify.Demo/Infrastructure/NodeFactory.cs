using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;

namespace Utility.Nodify.Demo
{
    public class NodeFactory : IFactory<IViewModelTree>
    {
        public IViewModelTree Create(object config)
        {
            var node = new Utility.Nodes.NodeViewModel() { Data = config };
            return node;
        }
    }
}
