using DynamicData;
using System;

namespace Utility.ViewModels.Filters;

public abstract class ObserverFilter<T> : Filter, IObserver<IChangeSet<T>>
{
    protected ObserverFilter(string header) : base(header)
    {
    }

    public virtual void OnCompleted()
    {
        //throw new NotImplementedException();
    }

    public virtual void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public abstract void OnNext(IChangeSet<T> value);
}
