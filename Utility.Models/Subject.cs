using System.Reactive.Subjects;
using Utility.Observables;

namespace Utility.Models
{
    public class Subject<T> : ISubject<T>
    {
        public Subject()
        {
        }

        public List<IObserver<T>> Observers { get; } = new();

        public List<T> Messages { get; } = new();

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public virtual void OnNext(T value)
        {
            throw new NotImplementedException();
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            foreach (var message in Messages.ToArray())
                observer.OnNext(message);
            return new Disposer<T>(Observers, observer);
        }
    }
}