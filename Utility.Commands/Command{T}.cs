using System.Windows.Input;

namespace Utility.Commands
{
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
