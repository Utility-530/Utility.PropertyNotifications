using NetPrints.Interfaces;

namespace NetPrints.Graph
{
    public interface IReturnNode : INode
    {

        INodeInputExecPin ReturnPin { get; }
        void AddReturnType();


    }
}