using DryIoc;
using MintPlayer.ObservableCollection;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.Trees.Demo.Connections
{
    public class BootStrapper
    {

        public BootStrapper(Container container)
        {
            container.Register<ViewModel>();
            //container.Register<ViewModelService>();
        }
    }

    public readonly record struct Change(string Name, object Value);

    public record ViewModel : NotifyProperty, IName //, IObservable<Change>
    {
        private object? value;
        private ObservableCollection<ViewModel> children;
        ReplaySubject<Change> changes = new();

        public IDisposable Subscribe(IObserver<Change> observer)
        {
            return changes.Subscribe(observer);
        }

        public string Name { get; set; }

        public object Value
        {
            get
            {
                this.RaisePropertyCalled(value);
                return value;
            }

            set
            {
                this.value = value;
                this.RaisePropertyReceived(value);
            }
        }
    }


    public class Service : IName, IObservable<object>, IObserver<object>
    {
        List<IObserver<object>> observers = new();

        public string Name { get; set; }

        public Func<object, object> Func { get; set; }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(object value)
        {
            var output = Func.Invoke(value);
            foreach (var observer in observers)
            {
                observer.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return new Utility.Observables.Generic.Disposer<object>(observers, observer);
        }
    }
}
