using NetPrints.Core;
using NetPrints.Enums;

namespace NetPrints.Interfaces
{
    public interface IExecutionGraph : INodeGraph
    {
        IExecutionEntryNode EntryNode { get; }
        MemberVisibility Visibility { get; }
                
        IClassGraph Class { get; }
    }
}