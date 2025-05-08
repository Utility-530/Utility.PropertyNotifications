using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IChildren
    {
        IObservable<object> Children { get; }
    }
}
