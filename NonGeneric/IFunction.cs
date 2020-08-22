using System;

namespace UtilityInterface.NonGeneric
{
    public interface IFunction
    {
        Func<object, object> Function { get; }
    }
}
