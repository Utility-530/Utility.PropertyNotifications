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

        private Lazy<Dictionary<string, MethodConnector>> inValues;
        private MethodInfo method;
        private ParameterInfo[] parameters;
        public Action next;
        private bool isActive;
        private Exception exception;
        private object? instance;
        MethodConnector outValue;

        public static MethodNode Create(MethodInfo method, object? instance = null)
        {
            var node = new MethodNode();

            if (method.IsStatic == false)
            {
                node.instance = instance ??= Locator.Current.GetService(method.DeclaringType);
            }
            node.method = method;
            node.parameters = method.GetParameters();
            if (method.ReturnType != typeof(void))
            {
                node.outValue = new() { Key = "return" };
            }
            node.inValues = new(() =>

            node.parameters.Count() == 0 ? new Dictionary<string, MethodConnector>
            {
                { string.Empty, create() }
            } :
            node.parameters.ToDictionary(a => a.Name ?? throw new Exception("s e!"), param =>
            {
                var model = new MethodConnector { Key = param.Name, Parameter = param };
       
                model
                .Subscribe(value =>
                {
                    Action? undoaction = new(() => { });
                    bool contains = node.values.ContainsKey(param.Name);
                    var previousResult = node.OutValue?.Value;
                    if (node.values.TryGetValue(param.Name, out var oldValue))
                    {
                        undoaction = new Action(() =>
                        {
                            if (contains)
                                node.values[param.Name] = oldValue;
                            else
                                node.values.Remove(param.Name);
                            if (previousResult == null && node.OutValue != null)
                            {
                                node.OutValue.Value = previousResult;
                            }
                        });
                    }

                    node.RaisePropertyChanged(nameof(IsActive));
                    Globals.Resolver.Resolve<IPlaybackEngine>().OnNext(
                        new PlaybackAction(node,
                        () => _action(param.Name, value),
                        undoaction,
                        a => node.IsActive = a,
                        new Dictionary<string, object> {
                            { "Value", value },
                            { "Name", param.Name },
                            { "PreviousValue", previousResult }
                            }
                         )
                        { Name = param.Name });

                });

                if (param.DefaultValue != DBNull.Value)
                {
                    model.OnNext(param.DefaultValue);
                }
                return model;
            }));

            return node;

            // where the method has no parameters but it is still desirable to execute it
            MethodConnector create()
            {
                var model = new MethodConnector { };

                model.Subscribe(a =>
                {

                    Action? undoaction = new(() => { });

                    var previousResult = node.OutValue?.Value;


                    node.RaisePropertyChanged(nameof(IsActive));
                    Globals.Resolver.Resolve<IPlaybackEngine>().OnNext(
                        new PlaybackAction(node,
                        () =>
                        {
                            try
                            {
                                var result = node.Execute(node.values);
                                if (node.OutValue == null)
                                    return;
                                node.OutValue.Value = result;
                            }
                            catch (Exception ex)
                            {
                                node.Exception = ex;
                                Globals.Exceptions.OnNext(ex);
                            }
                        },
                        undoaction,
                        a => node.IsActive = a,
                        new Dictionary<string, object> {

                            { "PreviousValue", previousResult }
                            }
                         )
                        { });

                });
                return model;
            }


            void _action(string name, object value)
            {
                node.next?.Invoke();
                node.values[name] = value;
                if (node.values.Count == node.parameters.Length)
                {
                    try
                    {
                        var result = node.Execute(node.values);
                        if (node.OutValue == null)
                            return;
                        node.OutValue.Value = result;
                    }
                    catch (Exception ex)
                    {
                        node.Exception = ex;
                        Globals.Exceptions.OnNext(ex);
                    }

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

        public MethodConnector OutValue => outValue;

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
