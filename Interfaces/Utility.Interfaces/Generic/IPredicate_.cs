using System;

namespace Utility.Interfaces.Generic
{
    public interface IPredicate_<T>
    {
        Predicate<T> Filter { get; set; }
    }
}