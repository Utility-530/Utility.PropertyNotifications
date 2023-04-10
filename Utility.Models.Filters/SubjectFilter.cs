using System;
using System.Reactive;

namespace Utility.Models.Filters;

public abstract class SubjectFilter<TIn> : ObserverFilter<TIn>, IRefreshObservable
{
    protected SubjectFilter(string header) : base(header)
    {
    }

    public abstract IDisposable Subscribe(IObserver<Unit> observer);
}