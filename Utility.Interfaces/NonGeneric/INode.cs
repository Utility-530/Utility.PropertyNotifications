using System.Collections;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes.Abstractions
{
    public interface INode
    {

        IEquatable Key { get; }
        object Content { get; }

        INode Parent { get; set; }

        IObservable Children { get; }

        IEnumerable Ancestors { get; }
    }
}