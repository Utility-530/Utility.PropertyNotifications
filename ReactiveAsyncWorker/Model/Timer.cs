
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveAsyncWorker
{
    public class Timer<T> : IObserver<T>, IObservable<T> where T : ITimed
    {
        private readonly ISubject<T> inSubject = new ReplaySubject<T>();
        private readonly IObservable<T> outObservable;

        public Timer()
        {
            outObservable = inSubject.SelectMany(a =>
            {
                return Observable.Timer(a.Finish).Select(v => a);
            });
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            inSubject.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return outObservable.Subscribe(observer.OnNext);
        }
    }
}


