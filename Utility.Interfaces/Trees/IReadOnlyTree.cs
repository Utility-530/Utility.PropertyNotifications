using System;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.Abstractions
{
    public interface IReadOnlyTree : ITreeIndex, IEquatable<IReadOnlyTree>, IChildren, IData, IParent<IReadOnlyTree>, IKey, IValue, IType, IAsyncClone
    {
        int Depth { get; }
    }
}