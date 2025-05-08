using System;
using System.Collections;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Trees.Abstractions
{
    public interface IReadOnlyTree : IEquatable<IReadOnlyTree>, IItems, IData, IParent<IReadOnlyTree>, IKey, IValue, IType, IAsyncClone
    {
        int Depth { get; }
    }
}