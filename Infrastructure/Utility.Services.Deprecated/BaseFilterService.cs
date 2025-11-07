using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Utility.Services.Deprecated;

public class BaseFilterService<T> : IObserver<Func<T, bool>>, IFilterService<T>, IRefreshObserver
{
    protected readonly ReplaySubject<Unit> subject = new(1);
    private Func<T, bool>? value;

    public BaseFilterService()
    {
    }

    protected virtual bool Execute(T profile)
    {
        if (value == null || value.Invoke(profile))
        {
            return true;
        }
        return false;
    }

    public void OnCompleted()
    {
        //throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        //throw new NotImplementedException();
    }

    public void OnNext(Func<T, bool> value)
    {
        this.value = value;
        OnNext(Unit.Default);
    }

    public void OnNext(Unit value)
    {
        subject.OnNext(value);
    }

    public IDisposable Subscribe(IObserver<Func<T, bool>> observer)
    {
        return subject.Select(a => new Func<T, bool>(Execute)).Subscribe(observer.OnNext,
            e =>
            {
            },
            () =>
            {
            });
    }
}