using System;
using System.Collections.ObjectModel;
using Utility.Models;

namespace Utility.Trees.Demo.MVVM
{
    public class Observable<T> : ObservableCollection<IObserver<T>>, IObservable<T>
    {
        private readonly Func<IObserver<T>, IDisposable> action;

        public Observable(Func<IObserver<T>, IDisposable> action)
        {
            this.action = action;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return action.Invoke(observer);
        }
    }

    public class Observable2<T> : ObservableCollection<IObserver<T>>, IObservable<T>, IObserver<T>
    {
        public Collection<IObserver<T>> Collection { get; set; } = new();
        public IObserver<T> Observer { get; set; }

        public Observable2()
        {
          
        }

        public void OnNext(T value)
        {
            foreach (var x in Collection)
                x.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return new Disposer<T>(Collection, observer);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}

