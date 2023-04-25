using System.Collections;
using Utility.Interfaces.Generic;

namespace Utility.Observables.Generic
{
    public class Relay<T> : IRelay<T>
    {
        List<IObserver<T>> observers = new();

        public Relay()
        {
        }

        public IEnumerable<IObserver<T>> Observers => observers;

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
            Messages.Add(value);
        }


        public void Broadcast(T message)
        {
            Messages.Add(message);
            foreach (var observer in Observers)
                observer.OnNext(message);
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            foreach (var message in Messages.ToArray())
                observer.OnNext(message);
            return new Disposer<T>(observers, observer);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Messages.GetEnumerator();
        }
    }
}