using System.Reactive.Subjects;
using Utility.Entities;

namespace Utility.Services
{
    public class SelectionService : IObserver<SelectionInput>, IObservable<SelectionOutput>
    {
        ReplaySubject<SelectionInput> one = new();
        ReplaySubject<SelectionOutput> two = new();

        public SelectionService()
        {
            one.Subscribe(a =>
            {
                two.OnNext(new SelectionOutput(a.Value));
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

        public void OnNext(SelectionInput value)
        {
            one.OnNext(value);  
        }

        public IDisposable Subscribe(IObserver<SelectionOutput> observer)
        {
            return two.Subscribe(observer);
        }
        #endregion
    }
}
