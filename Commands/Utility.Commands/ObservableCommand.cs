using System.Collections;
using System.Windows.Input;
using Utility.Interfaces.NonGeneric;
using System.Reactive;
using Utility.Observables.NonGeneric;
using System.Collections.ObjectModel;

namespace Utility.Commands
{
    public class ObservableCommand : ICommand, IObservable, IObservable<object>, IObserver<bool>
    {
        private bool canExecute;
         List<IObserver> observers  = new();
         List<IObserver<object>> observers2  = new();
        private object? id;
        private readonly Action<IObserver<bool>> methodToExecute;

        public ObservableCommand(Action<IObserver<bool>> methodToExecute, bool canExecute =true)
        {
            this.canExecute = canExecute;
            this.methodToExecute = methodToExecute;
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


        public IEnumerable<IObserver> Observers => observers;
        public IEnumerable<IObserver<object>> Observers2 => observers2;

        public List<object?> Outputs { get; } = new();

        public bool CanExecute(object? parameter) => canExecute;

        public void Execute(object? parameter)
        {
            var output = id ?? parameter;

            methodToExecute.Invoke(this);

            Outputs.Add(output);

            foreach (var observer in Observers)
            {
                observer.OnNext(id ?? parameter);
            }
        }

        public IDisposable Subscribe(IObserver observer)
        {
            foreach (var output in Outputs)
                observer.OnNext(output);
            return new Disposer(observers, observer);
        }

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
            return new Disposer<object>(observers2, observer);
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