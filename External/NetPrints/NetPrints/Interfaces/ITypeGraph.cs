using NetPrints.Core;
using NetPrints.Interfaces;

namespace NetPrints.Graph
{
    public interface ITypeGraph : INodeGraph, IReturnType
    {
        ITypeReturnNode ReturnNode { get; }
    }

    public interface IReturnType
    {
        ITypeSpecifier ReturnType { get; }

    }
}