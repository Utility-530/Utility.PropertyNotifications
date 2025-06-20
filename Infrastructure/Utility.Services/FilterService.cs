using Splat;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Entities;
using Utility.Interfaces.NonGeneric;

namespace Utility.Services
{
    public class FilterService : IObserver<Filter>, IObserver<InList> , IObservable<FilterPredicate>
    {

        ReplaySubject<Filter> one = new(1);
        ReplaySubject<InList> inlist = new(1);
        ReplaySubject<FilterPredicate> outList = new();

        public FilterService()
        {
            inlist.CombineLatest(one).Subscribe(a =>
            {
                var predicate = new Predicate<object>((v) => Locator.Current.GetService<IFilter>().Filter(new FilterQuery(a.Second.Value, v)));
                outList.OnNext(new FilterPredicate(predicate));
            });
        }

        #region boilerplate
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Filter value)
        {
            one.OnNext(value);

        }

        public void OnNext(InList value)
        {
            inlist.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<FilterPredicate> observer)
        {
            return outList.Subscribe(observer);
        }
        #endregion
    }
}
