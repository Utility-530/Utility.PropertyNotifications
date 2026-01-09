using NetPrints.Interfaces;

namespace NetPrints.Graph
{
    public interface ITypeReturnNode : INode
    {
        ITypeSpecifier InferredType { get; set; }
        INodeInputTypePin TypePin { get; }
    }
}