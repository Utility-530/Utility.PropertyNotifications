using System.Collections;
using Utility.Interfaces.NonGeneric;
using Utility.Interfaces.Reactive.NonGeneric;

namespace Utility.Observables.NonGeneric
{
    public class Subject : ISubject
    {
        private List<IObserver> observers = new();
        private readonly List<IDisposable> disposables = new();

        public IEnumerable<IObserver> Observers => observers;

        public List<object> Observations { get; } = new();

        public Subject()
        {
        }

        public Subject(params IDisposable[] disposables)
        {
            this.disposables.AddRange(disposables);
        }

        public void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

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
            foreach (var observer in Observers)
                observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            foreach (var observer in Observers)
                observer.OnError(error);
        }

        public IEnumerator GetEnumerator()
        {
            return Observations.GetEnumerator();
        }

        public IDisposable Subscribe(IObserver value)
        {
            var composite = new CompositeDisposable();
            composite.AddRange(disposables);
            composite.Add(new Disposer(observers, value));
            return composite;
        }

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }

        public void OnProgress(int i, int total)
        {
            foreach (var observer in Observers)
                observer.OnProgress(i, total);
        }
    }
}