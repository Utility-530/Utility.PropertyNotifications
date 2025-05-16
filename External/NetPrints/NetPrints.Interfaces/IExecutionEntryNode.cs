using NetPrints.Interfaces;

namespace NetPrints.Core
{
    public interface IExecutionEntryNode : INode
    {
        INodeOutputExecPin InitialExecutionPin { get; }
    }
}