using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leepfrog.WpfFramework.Commands
{
    public class StepRelayCommand : AsyncRelayCommand
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        private Boolean _isComplete;
        public Boolean IsComplete
        {
            get { return _isComplete; }
            private set
            {
                if (_isComplete != value)
                {
                    _isComplete = value;
                    RaisePropertyChanged("IsComplete");
                }
            }
        }

        #region Constructors
        public StepRelayCommand(string text, Func<object, Task> execute)
            : base(execute)
        {
            _text = text;
        }
        public StepRelayCommand(string text, Func<Task> execute)
            : this(text, async param => await execute())
        {
        }
        #endregion // Constructors

        public async override void Execute(object parameter)
        {
            IsProcessing = true;
            await _execute(parameter);
            IsProcessing = false;
            IsComplete = true;
        }
    }
}
