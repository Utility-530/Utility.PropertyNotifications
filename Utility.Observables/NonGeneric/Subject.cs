using System.Collections;
using Utility.Interfaces.NonGeneric;

namespace Utility.Observables.NonGeneric
{
    public class Subject : ISubject
    {
        List<IObserver> observers = new();
        public IEnumerable<IObserver> Observers => observers;

        public List<object> Observations { get; } = new();


        public virtual void OnNext(object value)
        {
            Observations.Add(value);
            foreach (var observer in Observers)
                observer.OnNext(value);
        }

        protected void Broadcast(object obj)
        {
            foreach (var observer in observers)
                observer.OnNext(obj);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return Observations.GetEnumerator();
        }

        public IDisposable Subscribe(IObserver value)
        {
            return new Disposer(observers, value);
        }

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }
    }
}
