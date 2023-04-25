using System.Collections;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Abstractions
{
    public interface INode
    {
        object Content { get; }

        INode Parent { get; set; }

        IObservable Children { get; }

        IEnumerable Ancestors { get; }
    }
}