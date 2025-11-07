using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IEquatable : IEquatable<IEquatable>
    {
    }
}