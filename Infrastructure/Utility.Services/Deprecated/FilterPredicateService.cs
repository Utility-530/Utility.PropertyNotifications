using Utility.Interfaces.NonGeneric;

namespace Utility.Services.Deprecated;

public class FilterPredicateService<T> : IObserver<IPredicate>, IFilterService<T>
{
    private readonly BaseFilterService<T> filterBaseService;

    public FilterPredicateService()
    {
        filterBaseService = new BaseFilterService<T>();
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(IPredicate predicate)
    {
        filterBaseService.OnNext(new Func<T, bool>(value =>
        {
            if (predicate.Evaluate(value) == false)
            {
                return true;
            }
            return false;
        }));
    }

    public IDisposable Subscribe(IObserver<Func<T, bool>> observer)
    {
        return filterBaseService.Subscribe(observer);
    }
}