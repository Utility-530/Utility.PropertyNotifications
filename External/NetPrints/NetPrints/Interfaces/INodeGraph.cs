using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.Interfaces
{
    public interface INodeGraph
    {
        IObservableCollection<INode> Nodes { get; }
        IClassGraph Class { get; }
    }
}
