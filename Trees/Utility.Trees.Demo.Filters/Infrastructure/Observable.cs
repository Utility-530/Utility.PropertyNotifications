using System;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.Models;
using Utility.PropertyNotifications;

namespace Utility.Trees.Demo.Filters
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

    public record Subject<T> : NotifyProperty, IObservable<T>, IObserver<T>, IObservable, IObserver
    {
        private int count;

        public Collection<IObserver<T>> Collection { get; set; } = new();
        public ObservableCollection<object> Values { get; set; } = new();

        public int Count { get => count; set => count = value; }

        public IObserver<T> Observer { get; set; }

        public Subject()
        {

        }

        public void OnNext(T value)
        {
            //if (Collection.Any() == false)
            //{
            //}
            Values.Add(value);
            if (Values.Count > 1)
            {

            }

            foreach (var x in Collection.ToArray())
                x.OnNext(value);
        }

        public void OnNext(object value)
        {    
            Values.Add(value);
            if (Values.Count > 1)
            {
            }

            if (value is T t)
            {
                foreach (var x in Collection.ToArray())
                {
                    x.OnNext(t);
                }
            }
            else
                throw new Exception(" SEFeee");

        }

        public IDisposable Subscribe(IObserver<T> observer)
        {

            var disposer = new Disposer<T>(Collection, observer);
            if (Collection.Count > 1)
            {

            }
            return disposer;
        }

        public IDisposable Subscribe(IObserver observer)
        {
            var disposer = new Disposer(Collection, observer);
            if (Collection.Count > 1)
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

