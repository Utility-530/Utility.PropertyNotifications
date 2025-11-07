using System;

namespace Utility.Interfaces.Generic
{
    public interface IFunction<T, R>
    {
        Func<T, R> Function { get; }
    }
}