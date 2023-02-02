using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Utility.Observables;

namespace Utility.WPF.Commands
{
    public class ObservableCommand : ICommand, IObservable<object?>, IObserver<bool>
    {
        private bool canExecute;
        private readonly object? id;


        public ObservableCommand(object? id = null)
        {
            this.id = id;
        }


        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public List<IObserver<object?>> Observers { get; } = new();
        public List<object?> Outputs { get; } = new();

        public bool CanExecute(object? parameter) => canExecute;

        public void Execute(object? parameter)
        {
            var output = id ?? parameter;
            Outputs.Add(output);

            foreach (var observer in Observers)
            {
                observer.OnNext(id ?? parameter);
            }
        }

        public IDisposable Subscribe(IObserver<object?> observer)
        {
            foreach (var output in Outputs)
                observer.OnNext(output);
            return new Disposer<object>(Observers, observer);
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
        }
    }

    public class ObservableCommand<T> : ICommand
    {

        private readonly Action<T?>? execute = null;
        private readonly Predicate<T?>? canExecute = null;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The Utility.Enums.Execution logic.</param>
        public ObservableCommand(Action<T?> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command with conditional Utility.Enums.Execution.
        /// </summary>
        /// <param name="execute">The Utility.Enums.Execution logic.</param>
        /// <param name="canExecute">The Utility.Enums.Execution status logic.</param>
        public ObservableCommand(Action<T?>? execute, Predicate<T?>? canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }


        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke((T?)parameter) ?? true;
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

        public void Execute(object? parameter)
        {
            execute?.Invoke((T?)parameter);
        }
    }
}
