using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IFunction
    {
        Func<object, object> Function { get; }
    }
}
