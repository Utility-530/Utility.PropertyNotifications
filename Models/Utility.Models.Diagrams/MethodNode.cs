using DryIoc;
using Splat;
using System.Reflection;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using IMethod = Utility.Interfaces.Methods.IMethod;

namespace Utility.Models.Diagrams
{
    public class MethodNode : NotifyPropertyClass, IResolvableNode, IMethod
    {

        readonly Dictionary<string, object> values = new();

        private readonly Lazy<Dictionary<string, MethodConnector>> inValues;
        private MethodInfo method;
        private ParameterInfo[] parameters;
        public Action next;
        private bool isActive;
        private Exception exception;
        private object? instance;

        public MethodNode(MethodInfo method, object? instance = null)
        {
            if (method.IsStatic == false)
            {
                this.instance = instance ??= Locator.Current.GetService(method.DeclaringType);
            }
            this.method = method;
            parameters = method.GetParameters();
            if (method.ReturnType != typeof(void))
            {
                OutValue = new() { Key = "return" };
            }
            inValues = new(() =>

            parameters.Count() == 0 ? new Dictionary<string, MethodConnector>
            {
                { string.Empty, create() }
            } :
            parameters.ToDictionary(a => a.Name ?? throw new Exception("s e!"), a =>
            {
                var model = new MethodConnector { Key = a.Name, Parameter = a };
                model
                .Subscribe(value =>
                {
                    Action? undoaction = new(() => { });
                    bool contains = values.ContainsKey(a.Name);
                    var previousResult = OutValue?.Value;
                    if (values.TryGetValue(a.Name, out var oldValue))
                    {
                        undoaction = new Action(() =>
                        {
                            if (contains)
                                values[a.Name] = oldValue;
                            else
                                values.Remove(a.Name);
                            if (previousResult == null && OutValue != null)
                            {
                                OutValue.Value = previousResult;
                            }
                        });
                    }

                    RaisePropertyChanged(nameof(IsActive));
                    Globals.Resolver.Resolve<IPlaybackEngine>().OnNext(
                        new PlaybackAction(this,
                        () => _action(a.Name, value),
                        undoaction,
                        a => IsActive = a,
                        new Dictionary<string, object> {
                            { "Value", value },
                            { "Name", a.Name },
                            { "PreviousValue", previousResult }
                            }
                         )
                        { Name = a.Name });

                });
                return model;
            }));

            // where the method has no parameters but it is still desirable to execute it
            MethodConnector create()
            {
                var model = new MethodConnector { };

                model.Subscribe(a =>
                {

                    Action? undoaction = new(() => { });
           
                    var previousResult = OutValue?.Value;       
            

                    RaisePropertyChanged(nameof(IsActive));
                    Globals.Resolver.Resolve<IPlaybackEngine>().OnNext(
                        new PlaybackAction(this,
                        () =>{
                            try
                            {
                                var result = this.Execute(values);
                                if (OutValue == null)
                                    return;
                                OutValue.Value = result;
                            }
                            catch (Exception ex)
                            {
                                Exception = ex;
                                Globals.Exceptions.OnNext(ex);
                            }
                        },
                        undoaction,
                        a => IsActive = a,
                        new Dictionary<string, object> {
                          
                            { "PreviousValue", previousResult }
                            }
                         )
                        {  });

                });
                return model;
            }
        }

        public void _action(string name, object value)
        {
            next?.Invoke();
            values[name] = value;
            if (values.Count == parameters.Length)
            {
                try
                {
                    var result = this.Execute(values);
                    if (OutValue == null)
                        return;
                    OutValue.Value = result;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    Globals.Exceptions.OnNext(ex);
                }

            }
        }

        public bool IsActive { get => isActive; set => this.RaisePropertyChanged(ref isActive, value); }
        public Exception Exception { get => exception; set => this.RaisePropertyChanged(ref exception, value); }

        public Dictionary<string, MethodConnector> InValues => inValues.Value;

        public object this[string index]
        {
            set => InValues[index].Value = value;
        }

        public MethodConnector OutValue { get; }

        public object? Instance => instance;
        public MethodInfo MethodInfo => method;
        public string Name => method.Name;
        public IReadOnlyCollection<ParameterInfo> Parameters => parameters;

        public override bool Equals(object? obj)
        {
            if (obj is MethodNode mNode)
                return this.MethodInfo.Equals(mNode.MethodInfo);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return MethodInfo.GetHashCode();
        }
    }

}
