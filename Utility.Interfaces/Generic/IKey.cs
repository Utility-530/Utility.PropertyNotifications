using System;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Generic
{
    public interface IKey<T> : IEquatable<IKey<T>>
    {
        T Key { get; }
    }

}
