using System.Windows.Input;

namespace Utility.Commands
{
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
}