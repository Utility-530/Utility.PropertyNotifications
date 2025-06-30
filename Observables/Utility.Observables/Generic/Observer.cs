
using Utility.Interfaces.NonGeneric;

namespace Utility.Observables.Generic
{
    public class Observer<T> : Interfaces.Reactive.IObserver<T>
    {
        private readonly Action<T> onNext;
        private readonly Action<Exception> onError;
        private readonly Action onCompleted;
        private readonly Action<int, int> onProgress;

        public Observer(Action<T> onNext, Action<Exception> onError, Action onCompleted, Action<int, int> onProgress)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
            this.onProgress = onProgress;
        }

        public Observer()
        {
        }

        public List<object> Observations { get; } = new();

        public virtual void OnNext(T value)
        {
            (onNext ?? throw new NotImplementedException()).Invoke(value);
        }

        public void OnCompleted()
        {
            (onCompleted ?? throw new NotImplementedException()).Invoke();
        }

        public void OnError(Exception error)
        {
            (onError ?? throw new NotImplementedException()).Invoke(error);
        }

        public void OnProgress(int amount, int total)
        {
            (onProgress ?? throw new NotImplementedException()).Invoke(amount, total);
        }

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }

        public void OnStarted()
        {
            throw new NotImplementedException();
        }
    }
}
