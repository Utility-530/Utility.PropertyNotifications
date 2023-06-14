using Utility.Interfaces.NonGeneric;

namespace Utility.Service;

public class FilterPredicateService<T> : IObserver<IPredicate>, IFilterService<T>
{
    private readonly FilterService<T> filterBaseService;

    public FilterPredicateService()
    {
        filterBaseService = new FilterService<T>();
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
            if (predicate.Invoke(value) == false)
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