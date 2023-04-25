using System.Collections;
using Utility.Interfaces.NonGeneric;

namespace Utility.Observables.NonGeneric
{
    public class Observer : IObserver
    {
        private Action<object> onNext;
        private Action<Exception> onError;
        private Action onCompleted;

        public Observer(Action<object> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        public Observer()
        {

        }

        public List<object> Observations { get; } = new();


        public virtual void OnNext(object value)
        {
            Observations.Add(value);
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


        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }
    }
}
