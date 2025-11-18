
using System.Windows.Input;

namespace PixelLab.Common
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private readonly Action methodToExecute;
        private readonly Func<bool>? canExecuteEvaluator;

        public DelegateCommand(Action methodToExecute, Func<bool>? canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        public DelegateCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }

        public DelegateCommand(Func<Task> methodToExecute, Func<bool>? canExecuteEvaluator = null)
            : this(new Action(async () => await methodToExecute()), canExecuteEvaluator)
        {
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
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

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T?>? execute = null;
        private readonly Predicate<T?>? canExecute = null;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The Utility.Enums.Execution logic.</param>
        public DelegateCommand(Action<T?> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command with conditional Utility.Enums.Execution.
        /// </summary>
        /// <param name="execute">The Utility.Enums.Execution logic.</param>
        /// <param name="canExecute">The Utility.Enums.Execution status logic.</param>
        public DelegateCommand(Action<T?>? execute, Predicate<T?>? canExecute)
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
