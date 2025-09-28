using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IChanges
    {
        IObservable<object> Changes { get; }
    }
}
