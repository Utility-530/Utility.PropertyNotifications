using System.Windows.Input;
using Utility.Models;
using Utility.Observables.Generic;

namespace Utility.Commands
{
    public class ObservableCommand<T> : ICommand, IObservable<T>, IObserver<bool>
    {
        private T? id;
        private bool canExecute;
        private readonly Action<IObserver<bool>> methodToExecute;

        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The Utility.Enums.Execution logic.</param>
        public ObservableCommand(T? id = default, bool canExecute = true)
        {
            this.id = id;
            this.canExecute = canExecute;
        }

        public ObservableCommand(Action<IObserver<bool>> methodToExecute)
        {
            this.methodToExecute = methodToExecute;
        }


        public event EventHandler? CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public List<IObserver<T?>> Observers { get; } = new();

        public List<T?> Outputs { get; } = new();

        public bool CanExecute(object? parameter) => canExecute;

        public void Execute(object? parameter)
        {
            T? output = id ?? (T)parameter;
            methodToExecute.Invoke(this);
            Outputs.Add(output);

            foreach (var observer in Observers)
            {
                observer.OnNext(output);
            }
        }

        public IDisposable Subscribe(IObserver<T?> observer)
        {
            foreach (var output in Outputs)
                observer.OnNext(output);
            return new Disposer<T>(Observers, observer);
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
    }
}