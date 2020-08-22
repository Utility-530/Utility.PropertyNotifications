using System;

namespace UtilityInterface.Generic
{
    public interface IFunction<T, R>
    {
        Func<T,R> Function { get; }
    }

}
