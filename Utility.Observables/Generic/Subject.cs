using System.Collections;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.NonGeneric;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utility.Observables.Generic
{

    public class Subject<TInputOutput> : Subject<TInputOutput, TInputOutput>
    {
        public Subject() : base(a => a)
        {
        }
    }

    public interface IIOType 
    {
        public Type InType { get; }
        public Type OutType { get; }
    }


    public class Subject<TInput, TOutput> : Interfaces.Generic.IObserver<TInput>, Interfaces.Generic.IObservable<TOutput>
    {
        private Func<TInput, TOutput> onNext;

        List<Interfaces.Generic.IObserver<TOutput>> _observers = new();
        List<TOutput> outputs = new();
        List<IDisposable> disposables = new();
        public IEnumerable<Interfaces.Generic.IObserver<TOutput>> Observers => _observers;

        public Subject(Func<TInput, TOutput> onNext)
        {
            this.onNext = onNext;
        }

        public Type InType => typeof(TInput);   
        public Type OutType => typeof(TOutput);   

        public virtual void OnNext(TInput value)
        {
            var output = (onNext ?? throw new NotImplementedException()).Invoke(value);
            outputs.Add(output);
            foreach (var observer in _observers.ToArray())
            {
                observer.OnNext(output);
            }
        }

        public void OnCompleted()
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.OnError(error);
            }
        }
        public void OnProgress(int complete, int total)
        {
            foreach (var observer in _observers.ToArray())
            {
                observer.OnProgress(complete, total);
            }
        }

        public bool Equals(IEquatable? other)
        {
            throw new NotImplementedException();
        }


        public IDisposable Subscribe(Interfaces.Generic.IObserver<TOutput> observer)
        {
            foreach (var output in outputs)
            {
                observer.OnNext(output);
            }
            return new Disposer<Interfaces.Generic.IObserver<TOutput>, TOutput>(_observers, observer)
                .DisposeWith(new CompositeDisposable(disposables));
        }

        public override string ToString()
        {
            return typeof(TInput).Name + " ~ " + typeof(TOutput).Name;
        }



        public IEnumerator GetEnumerator()
        {
            return outputs.GetEnumerator();
        }

        internal void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }
    }
}
