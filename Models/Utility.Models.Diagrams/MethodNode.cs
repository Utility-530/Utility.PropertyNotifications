using Utility.Interfaces.Exs;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using IMethod = Utility.Interfaces.Exs.IMethod;
using Utility.Extensions;

namespace Utility.Models.Diagrams
{
    public class MethodNode : NotifyPropertyClass, IResolvableNode
    {

        readonly Dictionary<string, object> names = new();

        private readonly Lazy<Dictionary<string, MethodConnector>> inValues;

        public MethodNode(IMethod method)
        {
            Method = method;
            var parameters = method.MethodInfo.GetParameters();
            inValues = new(() => parameters.ToDictionary(a => a.Name ?? throw new Exception("s e!"), a =>
            {
                var model = new MethodConnector { Key = Guid.NewGuid().ToString() };
                model
                .Subscribe(value =>
                {
                    bool contains = names.ContainsKey(a.Name);
                    var previousResult = OutValue.Value;
                    var action = new Action(() =>
                    {
                        names[a.Name] = value;
                        if (names.Count == parameters.Length)
                        {
                            var result = method.Execute(names);
                            OutValue.Value = result;
                        }
                    });

                    Action? undoaction = new(() => { });
                    if (names.TryGetValue(a.Name, out var oldValue))
                    {
                        undoaction = new Action(() =>
                        {
                            if (contains)
                                names[a.Name] = oldValue;
                            else
                                names.Remove(a.Name);
                            if (previousResult == null)
                            {
                                OutValue.Value = previousResult;
                            }
                            //if (names.Count == parameters.Length)
                            //{
                            //    var result = method.Execute(names);
                            //    OutValue.Value = result;
                            //}
                        });
                    }
                    IsActive = true;
                    RaisePropertyChanged(nameof(IsActive));
                    Globals.Resolver.Resolve<IPlaybackEngine>().OnNext(new MethodAction(this, action, undoaction));

                });
                return model;
            }));
        }

        public bool IsActive { get; set; }

        public Dictionary<string, MethodConnector> InValues => inValues.Value;

        public object this[string index]
        {
            set => InValues[index].Value = value;
        }

        public MethodConnector OutValue { get; } = new() { Key = Guid.NewGuid().ToString() };

        public IMethod Method { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is MethodNode mNode)
                return Method.Equals(mNode.Method);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Method.GetHashCode();
        }
    }

}
