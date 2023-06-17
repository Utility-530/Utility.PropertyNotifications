using System.Collections;
using Utility.Interfaces.NonGeneric;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utility.Observables.NonGeneric
{
    public class Observer : IObserver
    {
        private Action<object> onNext;
        private Action<Exception> onError;
        private Action onCompleted;
        private readonly Action<int, int> onProgress;

        public Observer(Action<object> onNext, Action<Exception> onError, Action onCompleted, Action<int,int> onProgress)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
            this.onProgress = onProgress;
        }

        // Used by disposer
        internal Observer()
        {

        }

        public List<object> Observations { get; } = new();


        public void OnNext(object value)
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

        public void OnProgress(int i, int total)
        {
            (onProgress ?? throw new NotImplementedException()).Invoke(i, total);
        }

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }

    }
}
