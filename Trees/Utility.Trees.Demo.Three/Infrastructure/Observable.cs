using System;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.Helpers.NonGeneric;
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
            if(Collection.Any()==false)
            {

            }
            foreach (var x in Collection.ToArray())
                x.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
     
            var disposer = new Disposer<T>(Collection, observer);
            if (Collection.Any() == false)
            {

            }
            return disposer;
        }

        public void OnCompleted()
        {
            //throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}

