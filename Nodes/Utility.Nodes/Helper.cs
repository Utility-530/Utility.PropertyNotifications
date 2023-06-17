using Utility.Nodes.Abstractions;

namespace Utility.Nodes
{
    public static class NodesHelper
    {
        public static INode FindAncestor(this INode valueNode, Predicate<INode> predicate)
        {
            INode parent = valueNode;
            while (parent != null && predicate(parent) == false)
            {
                parent = parent.Parent;
            }
            return parent;
        }
    }
}
