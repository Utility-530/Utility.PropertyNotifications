using System.Collections;
using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.Abstractions
{
    public interface IReadOnlyTree : IEquatable<IReadOnlyTree>
    {
        [Obsolete]
        IEquatable Key { get; }

        IReadOnlyTree Parent { get; set; }

        IEnumerable Items { get; }

        object Data { get; }
    }
}