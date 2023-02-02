namespace Utility.Commands
{
    using System;
    using System.Windows.Input;


    public class Command : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private readonly Action methodToExecute;
        private readonly Func<bool>? canExecuteEvaluator;

        public Command(Action methodToExecute, Func<bool>? canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        public Command(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }

        public bool CanExecute(object? parameter)
        {
            if (canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = canExecuteEvaluator.Invoke();
                return result;
            }
        }

        public void Execute(object? parameter)
        {
            methodToExecute.Invoke();
        }
    }

    public class Command<T> : ICommand
    {

        private readonly Action<T?>? execute = null;
        private readonly Predicate<T?>? canExecute = null;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The Utility.Enums.Execution logic.</param>
        public Command(Action<T?> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command with conditional Utility.Enums.Execution.
        /// </summary>
        /// <param name="execute">The Utility.Enums.Execution logic.</param>
        /// <param name="canExecute">The Utility.Enums.Execution status logic.</param>
        public Command(Action<T?>? execute, Predicate<T?>? canExecute)
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
