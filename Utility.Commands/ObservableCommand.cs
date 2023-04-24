using System.Collections;
using System.Windows.Input;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.NonGeneric;

namespace Utility.Commands
{
    public class ObservableCommand : ICommand, IObservable
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

         List<IObserver> observers  = new();
        public IEnumerable<IObserver> Observers => observers;

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

        public IDisposable Subscribe(IObserver observer)
        {
            foreach (var output in Outputs)
                observer.OnNext(output);
            return new Disposer(observers, observer);
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

        public IEnumerator GetEnumerator()
        {
            return Outputs.GetEnumerator();
        }
    }
}