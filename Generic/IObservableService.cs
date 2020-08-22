using System;

namespace UtilityInterface.Generic
{
    public interface IObservableService<R>
    {
        IObservable<R> Observable { get; }
    }

}
