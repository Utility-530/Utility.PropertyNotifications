using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IReactiveProperty : IValue, IObservable<object>, IObserver<object>
    {
    }
}