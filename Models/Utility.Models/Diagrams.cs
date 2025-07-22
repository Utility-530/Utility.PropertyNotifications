using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Helpers;

namespace Utility.Models
{
    public class MethodConnector : ReactiveProperty<object>, IKey
    {
        public string Key { get; set; }
    }

    public class MethodAction : IAction
    {
        private Action action;
        private Action undoaction;

        public MethodAction(MethodNode methodNode, Action action, Action undoaction)
        {
            MethodNode = methodNode;
            this.action = action;
            this.undoaction = undoaction;
        }

        public MethodNode MethodNode { get; }

        public void Do()
        {
            action();
        }

        public void Undo()
        {
            undoaction();
        }
    }

    public class MethodNode : NotifyPropertyClass, IResolvableNode
    {

        readonly Dictionary<string, object> names = new();

        private readonly Lazy<Dictionary<string, MethodConnector>> inValues;

        public MethodNode(Method method)
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

                    Locator.Current.GetService<IPlaybackEngine>().OnNext(new MethodAction(this, action, undoaction));

                });
                return model;
            }));
        }

        public Dictionary<string, MethodConnector> InValues => inValues.Value;

        public object this[string index]
        {
            set => InValues[index].Value = value;
        }

        public MethodConnector OutValue { get; } = new() { Key = Guid.NewGuid().ToString() };

        public Method Method { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is MethodNode mNode)
                return this.Method.Equals(mNode.Method);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Method.GetHashCode();
        }
    }
    public interface IResolvableNode
    {

    }

    public interface IResolvableConnection
    {

    }

    public class ObservableNode(string name, IObservable<object> observable, IValueModel valueModel) : IResolvableNode
    {
        public string Name => name;
        public IObservable<object> Connector { get; } = observable;
        public IValueModel ValueModel { get; } = valueModel;

        public override bool Equals(object? obj)
        {
            if (obj is ObservableNode mNode)
                return this.Name.Equals(mNode.Name) && this.ValueModel.Equals(mNode.ValueModel);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.ValueModel.GetHashCode();
        }
    }

    public class ObserverNode(string name, IObserver<object> observer, IValueModel valueModel) : IResolvableNode
    {
        public IObserver<object> Connector { get; } = observer;
        public IValueModel ValueModel { get; } = valueModel;

        public string Name => name;

        public override bool Equals(object? obj)
        {
            if (obj is ObserverNode mNode)
                return this.Name.Equals(mNode.Name) && this.ValueModel.Equals(mNode.ValueModel);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.ValueModel.GetHashCode();
        }
    }


    public record MethodConnection(IObserver<object> In, IObservable<object> Out, IDisposable Disposable) : IResolvableConnection;

}
