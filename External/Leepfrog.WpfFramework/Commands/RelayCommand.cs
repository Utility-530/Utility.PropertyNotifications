using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Leepfrog.WpfFramework.ViewModel;
using System.ComponentModel;

namespace Leepfrog.WpfFramework.Commands
{
    public class RelayCommand : ICommand, INotifyPropertyChanged
    {
        protected readonly Action<object> _execute;
        protected readonly Predicate<object> _canExecute;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ButtonStyle { get; set; }

        #region Constructors
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if ( execute == null )
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }
        public RelayCommand(Action execute, Func<Boolean> canExecute = null)
            : this(param => execute(), param => canExecute())
        {
        }
        #endregion // Constructors

        #region ICommand Members
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanExecute)));
        }
        #endregion // ICommand Members
    }
}
