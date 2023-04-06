using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Utility.Observables;

namespace Utility.Commands
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
            CommandManager.InvalidateRequerySuggested();
        }
    }

 
}
