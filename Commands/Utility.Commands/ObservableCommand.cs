using System.Collections;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Utility.Commands
{
    public class ObservableCommand : ICommand, IObservable<object>, IObserver<bool>
    {
        private bool canExecute;
        private Func<IObserver<bool>, object> methodToExecute_;
        //List<IObserver> observers  = new();
        List<IObserver<object>> observers = new();
        private object? id;
        private readonly Action<IObserver<bool>> methodToExecute;

        public ObservableCommand(Action<IObserver<bool>> methodToExecute, bool canExecute = true)
        {
            this.canExecute = canExecute;
            this.methodToExecute = methodToExecute;
        }

        public ObservableCommand(Func<IObserver<bool>, object> methodToExecute, bool canExecute = true)
        {
            this.canExecute = canExecute;
            this.methodToExecute_ = methodToExecute;
        }


        public ObservableCommand(object? id = null)
        {
            this.id = id;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        //public IEnumerable<IObserver> Observers => observers;
        public IEnumerable<IObserver<object>> Observers => observers;

        public List<object?> Outputs { get; } = new();

        public bool CanExecute(object? parameter) => canExecute;

        public void Execute(object? parameter)
        {      

            if (methodToExecute != null)
            {
                methodToExecute.Invoke(this);

                var output = id ?? parameter;

                Outputs.Add(output);

                foreach (var observer in observers)
                {
                    observer.OnNext(output);
                }
            }
            else if(methodToExecute_!=null)
            {
                var output = methodToExecute_.Invoke(this);

                Outputs.Add(output);

                foreach (var observer in observers)
                {
                    observer.OnNext(output);
                }
            }
        }

        //public IDisposable Subscribe(IObserver observer)
        //{
        //    foreach (var output in Outputs)
        //        observer.OnNext(output);
        //    return new Disposer(observers, observer);
        //}

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(bool value)
        {
            canExecute = value;
            CommandManager.InvalidateRequerySuggested();
        }

        public IEnumerator GetEnumerator()
        {
            return Outputs.GetEnumerator();
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            foreach (var output in Outputs)
                observer.OnNext(output);
            return new Disposer<object>(observers, observer);
        }

        public class Disposer<TObserver, T> : IDisposable where TObserver : IObserver<T>
        {
            private readonly ICollection<TObserver> observers;
            private readonly TObserver observer;

            public Disposer(ICollection<TObserver> observers, TObserver observer)
            {
                (this.observers = observers).Add(observer);
                this.observer = observer;
            }

            public IEnumerable Observers => observers;
            public IObserver<T> Observer => observer;

            public void Dispose()
            {
                observers?.Remove(observer);
            }

            public static Disposer<TObserver, T> Empty => new(new Collection<TObserver>(), default);

        }

        public sealed class Disposer<T> : Disposer<IObserver<T>, T>
        {

            public Disposer(ICollection<IObserver<T>> observers, IObserver<T> observer) : base(observers, observer)
            {
            }
        }

    }
}