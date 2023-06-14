using System.Collections;
using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

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

    public abstract class Subject
    {
        public abstract IList Outputs { get; }
        public abstract IList Observers { get; }
    }

    public record Progress(int Amount, int Total);

    public class Subject<TInput, TOutput> : Subject, Interfaces.Generic.IObserver<TInput>, Interfaces.Generic.IObservable<TOutput>
    {
        private Func<TInput, TOutput> onNext;

        protected ObservableCollection<Interfaces.Generic.IObserver<TOutput>> _observers = new();

        public override ObservableCollection<TOutput> Outputs { get; } = new();

        List<IDisposable> disposables = new();
        public override ObservableCollection<Interfaces.Generic.IObserver<TOutput>> Observers => _observers;

        public bool IsCompleted { get; set; }

        public Exception? Exception { get; set; }

        public Progress? Progress { get; set; }

        public Subject(Func<TInput, TOutput> onNext)
        {
            this.onNext = onNext;
        }

        public virtual Type InType => typeof(TInput);

        public virtual Type OutType => typeof(TOutput);

        IEnumerable<Interfaces.Generic.IObserver<TOutput>> Interfaces.Generic.IObservable<TOutput>.Observers => _observers;

        public virtual void OnNext(TInput value)
        {
            var output = (onNext ?? throw new NotImplementedException()).Invoke(value);
            Outputs.Add(output);
            //if (_observers.Any() == false)
            //    throw new Exception("FDBd444 rrr");
            foreach (var observer in _observers.ToArray())
            {
                observer.OnNext(output);
            }
           
        }

        public void OnCompleted()
        {
            if(InType.Name=="ChildrenResponse")
            {
            
            }
            IsCompleted = true;
            //if (Outputs.Count > 0)
            //    Outputs.Clear();
            foreach (var observer in _observers.ToArray())
            {
                observer.OnCompleted();
            }
            _observers.Clear();
        }

        public void OnError(Exception error)
        {
            Exception = error;
            foreach (var observer in _observers.ToArray())
            {
                observer.OnError(error);
            }
        }
        public void OnProgress(int complete, int total)
        {
            Progress = new(complete, total);
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
            foreach (var output in Outputs)
            {
                observer.OnNext(output);
            }
            if (InType.Name == "ChildrenResponse" )
            {
                
            }
            if (IsCompleted) { observer.OnCompleted(); return Disposer<Interfaces.Generic.IObserver<TOutput>>.Empty; }
            if (Exception != null) { observer.OnError(Exception); }
            if (Progress != null) { observer.OnProgress(Progress.Amount, Progress.Total);}
            return new Disposer<Interfaces.Generic.IObserver<TOutput>, TOutput>(_observers, observer)
                .DisposeWith(new CompositeDisposable(disposables));
        }

        public override string ToString()
        {
            return typeof(TInput).Name + " ~ " + typeof(TOutput).Name;
        }



        public IEnumerator GetEnumerator()
        {
            return Outputs.GetEnumerator();
        }

        internal void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }
    }
}
