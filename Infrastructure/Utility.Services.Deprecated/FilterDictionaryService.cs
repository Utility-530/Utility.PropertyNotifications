using System.Reactive;

namespace Utility.Services.Deprecated;

public interface IFilterService<T> : IObservable<Func<T, bool>>
{
}

public interface IRefreshObserver : IObserver<Unit>
{
}

public class FilterDictionaryService<T> : IObserver<Dictionary<string, bool>>, IFilterService<T>
{
    private readonly BaseFilterService<T> filterBaseService;

    public FilterDictionaryService(Func<T, string> keyFunc)
    {
        KeySelector = keyFunc;
        filterBaseService = new BaseFilterService<T>();
    }

    public Func<T, string> KeySelector { get; }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(Dictionary<string, bool> dictionary)
    {
        filterBaseService.OnNext(new Func<T, bool>(value =>
        {
            if (dictionary.ContainsKey(KeySelector(value)) == false)
            {
                return true;
            }
            return dictionary[KeySelector(value)];
        }));
    }

    public IDisposable Subscribe(IObserver<Func<T, bool>> observer)
    {
        return filterBaseService.Subscribe(observer);
    }
}