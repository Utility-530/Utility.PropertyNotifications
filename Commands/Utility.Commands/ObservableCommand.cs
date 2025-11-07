using System.Collections;
using System.Windows.Input;
using Utility.Observables;

namespace Utility.Commands
{
    public class ObservableCommand : ICommand, IObservable<object>, IObserver<bool>
    {
        private bool canExecute;
        private Func<IObserver<bool>, object> methodToExecute_;
        private List<IObserver<object>> observers = new();
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
            else if (methodToExecute_ != null)
            {
                var output = methodToExecute_.Invoke(this);

                Outputs.Add(output);

                foreach (var observer in observers)
                {
                    observer.OnNext(output);
                }
            }
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
            return new Disposer<object>(observers, observer);
        }
    }
}