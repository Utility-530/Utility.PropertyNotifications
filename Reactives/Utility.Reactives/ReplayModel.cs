using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Utility.Reactives
{
    public class ReplayModel<T> : ISubject<T>
    {
        private readonly ObservableCollection<T> collection = new();

        private readonly List<IObserver<T>> observers = new();
        private readonly IScheduler scheduler;
        private int count = 0;

        public ReplayModel(IScheduler scheduler = null)
        {
            this.scheduler = scheduler;
        }

        public ICollection Collection => collection;

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
            Schedule(value);
            foreach (var observer in observers.ToArray())
            {
                observer.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (collection)
                foreach (var value in collection.ToArray())
                {
                    observer.OnNext(value);
                }
            var tempCount = ++count;
            observers.Add(observer);
            return Disposable.Create(() => observers.Remove(observer));
        }

        private void Schedule(T value)
        {
            if (scheduler != null)
            {
                scheduler.Schedule(() =>
                {
                    lock (collection)
                        collection.Add(value);
                });
            }
            else
            {
                lock (collection)
                    collection.Add(value);
            }
        }
    }
}