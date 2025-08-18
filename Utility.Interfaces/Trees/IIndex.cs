using System;
using System.Collections.Generic;

namespace Utility.Interfaces.Trees
{
    public interface IIndex: IReadOnlyList<int>, IComparable<IIndex>, IEquatable<IIndex>, IComparable
    {
        bool IsEmpty { get; }
        int Local { get; }
    }
}