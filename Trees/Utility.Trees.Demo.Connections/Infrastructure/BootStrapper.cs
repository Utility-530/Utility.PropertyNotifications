using DryIoc;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Pipes;
using Utility.PropertyNotifications;

namespace Utility.Trees.Demo.Connections
{
    public class BootStrapper
    {

        public BootStrapper()
        {
            //Locator.CurrentMutable.RegisterLazySingleton<ViewModel>(()=>new());
            Locator.CurrentMutable.RegisterLazySingleton<PipeController>(() => new());

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

        public MethodInfo MethodInfo { get; set; }
        public object Instance { get; set; }

        public ObservableCollection<Parameter> Inputs { get; set; }

        public ObservableCollection<Parameter> Outputs { get; set; }


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
            if (value is Change valueChange)
            {

                var input = Inputs.Single(a=>a.Name== valueChange.Name);
                input.Value = valueChange.Value;
                if(Inputs.All(a=>a.Value!=null))
                {
                    var output = MethodInfo.Invoke(Instance, Inputs.Select(i => i.Value).ToArray());
                    foreach (var observer in observers)
                    {
                        observer.OnNext(output);
                    }
                }
            }
            else
            {
                //var output = Func.Invoke(value);
                //foreach (var observer in observers)
                //{
                //    observer.OnNext(value);
                //}
            }
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return new Utility.Observables.Disposer<object>(observers, observer);
        }
    }

    public class Parameter
    {
        private readonly ParameterInfo parameterInfo;
        private readonly MethodInfo methodInfo;

        public Parameter(ParameterInfo parameterInfo, MethodInfo methodInfo)
        {
            this.parameterInfo = parameterInfo;
            this.methodInfo = methodInfo;
        }

        //public string Name => parameterInfo.Position == -1 ? "return" : parameterInfo.Name;
        public string Name => parameterInfo.Name == null ? "return" : parameterInfo.Name;
        public string ServiceName => methodInfo.Name;

        public object Value { get; set; }

    }
}
