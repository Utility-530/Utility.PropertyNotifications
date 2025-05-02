using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Leepfrog.WpfFramework.ViewModel;

namespace Leepfrog.WpfFramework.Commands
{
    public class AsyncRelayCommand : ICommand, INotifyPropertyChanged
    {

        protected bool _isProcessing;
        public bool IsProcessing
        {
            get
            { return _isProcessing; }
            protected set
            {
                if (_isProcessing != value)
                {
                    _isProcessing = value;
                    RaisePropertyChanged(nameof(IsProcessing));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        protected readonly Func<object,Task<bool>> _execute;
        protected readonly Predicate<object> _canExecute;

        #region Constructors
        public AsyncRelayCommand(Func<object, Task> execute, Predicate<object> canExecute = null)
            : this(async param => { await execute(param); return false; }, canExecute)
        {
        }
        public AsyncRelayCommand(Func<Task> execute, Func<Boolean> canExecute = null)
            : this(async param => await execute(), param => canExecute())
        {
        }
        public AsyncRelayCommand(Func<object, Task<bool>> execute, Predicate<object> canExecute = null)
        {
            //string msg = $"{this.GetType().Name} ({this.GetHashCode()}) Created";
            //System.Diagnostics.Debug.WriteLine(msg);

            if ( execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }
        public AsyncRelayCommand(Func<Task<bool>> execute, Func<Boolean> canExecute = null)
            : this(async param => await execute(), param => canExecute())
        {
        }
        #endregion // Constructors

        #region ICommand Members
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return (!_isProcessing) && (_canExecute == null ? true : _canExecute(parameter));
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public event EventHandler Finished;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async virtual void Execute(object parameter)
        {
            await ExecuteAsync(parameter).ConfigureAwait(false);
        }
        public async virtual Task ExecuteAsync(object parameter)
        {
            IsProcessing = true;
            bool latchProcessing = await _execute(parameter);//.ConfigureAwait(false); // changed ml 2019-01-09 - added configureawait - removed ml 2019-01-18 - IsProcessing can only be accessed from the dispatcher thread!
            if (!latchProcessing)
            {
                FinishedProcessing();
            }
        }
        #endregion // ICommand Members

        ~AsyncRelayCommand()
        {
            //string msg = $"{this.GetType().Name} ({this.GetHashCode()}) Finalized";
            //System.Diagnostics.Debug.WriteLine(msg);
        }

        public void FinishedProcessing()
        {
            IsProcessing = false;
            Finished?.Invoke(this, new EventArgs());
        }
    }
}
