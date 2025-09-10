using DryIoc;
using LanguageExt.Pipes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodify.Base;
using Utility.Nodify.Base.Abstractions;
using Utility.Nodify.Core;
using Utility.Nodify.Models;
using Utility.Reactives;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.Helpers.Generic;

namespace Utility.Nodify.Engine
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly DryIoc.IContainer container;

        public ViewModelFactory(DryIoc.IContainer container)
        {
            this.container = container;
        }

        public IConnectionViewModel CreateConnection(IConnectorViewModel source, IConnectorViewModel? target)
        {

            //if (target == null)
            //{
            //    PendingConnection.IsVisible = true;
            //    OpenAt(PendingConnection.TargetLocation);
            //    Menu.Closed += OnOperationsMenuClosed;
            //    return;
            //}

            var connectionViewModel = new ConnectionViewModel
            {
                Input = source,
                Output = target,
                Data = container.Resolve<IConnectionFactory>().CreateConnection(source.Data, target.Data),
                IsDirectionForward = source.Node?.Data is Tree && target.Node.Data?.GetType().Name.Contains("observable", StringComparison.InvariantCultureIgnoreCase) == true
            };

            return connectionViewModel;

            //container.RegisterInstanceMany<IConnectionViewModel>(connectionViewModel);
            //Connections.Add(connectionViewModel);
        }


        public bool CanCreateConnection(IConnectorViewModel source, IConnectorViewModel? target)
            => target == null || source != target && source.Node != target.Node && source.IsInput != target.IsInput;


        public IConnectorViewModel CreateConnector(object dataContext)
        {
            if (dataContext is Utility.Nodify.Base.ConnectorParameters { Guid: Guid guid, Data: var data, IsInput: bool isInput, Key: var key, Node: var node } s)
            {
                return new ConnectorViewModel()
                {
                    Guid = guid,
                    IsInput = isInput,
                    Key = key,
                    Node = node,
                    Data = data
                };
            }
            else if (dataContext is string _s)
            {
                return new ConnectorViewModel() { Data = dataContext, Key = _s };
            }

            return new ConnectorViewModel() { Data = dataContext, Key = "" };
        }

        public INodeViewModel CreateNode(object dataContext)
        {
            if(dataContext is InstanceKey instanceKey)
            {
                return new NodeViewModel() { Key = instanceKey.Key, Data = instanceKey.Data, Input = [], Output = [] };
            }
            else if (dataContext is IReadOnlyTree tree)
            {
                if(tree is NamedTree namedTree)
                {
                    return new NodeViewModel()
                    {
                        Data = tree.Data,
                        Guid = Guid.Parse((tree as IGetKey).Key),
                        Key = tree.Index.ToString(),
                        Input = [],
                        Output = []
                    };
                }
                var x = CreatePendingConnector(true);// new PendingConnectorViewModel() { IsInput = true };
                var y = CreatePendingConnector(null);
                var nodeViewModel = new NodeViewModel()
                {

                    Data = tree.Data,
                    Guid = Guid.Parse((tree as IGetKey).Key),
                    Key = tree.Index.ToString(),
                    Input = new CollectionWithFixedLast<IConnectorViewModel>(x),
                    Output = new CollectionWithFixedLast<IConnectorViewModel>(y)
                    //location = settings.nodelocationgenerator(settings, ++i),
                };
                x.Node = nodeViewModel;
                y.Node = nodeViewModel;
                return nodeViewModel;
            }
            //else if (dataContext is INotifyPropertyChanged npc)
            //{
            //    if (info.PropertyType == typeof(string))
            //    {
            //        observable = changed.WithChangesTo<INotifyPropertyChanged, string>(info);

            //    }
            //    else if (info.PropertyType == typeof(bool))
            //    {
            //        observable = changed.WithChangesTo<INotifyPropertyChanged, bool>(info);

            //    }
            //    else if (info.PropertyType == typeof(int))
            //    {
            //        observable = changed.WithChangesTo<INotifyPropertyChanged, int>(info);
            //    }

            //    var nodeViewModel = new NodeViewModel { Data = observable };

            //    var input = new ConnectorViewModel { Data = info, Key = "input" };
            //    var output = new ConnectorViewModel { Data = info, Key = "output" };
            //    nodeViewModel.Input.Add(input);
            //    nodeViewModel.Output.Add(output);
            //    input.Node = nodeViewModel;
            //    output.Node = nodeViewModel;
            //    return nodeViewModel;
            //}
            else if (dataContext is MethodInfo methodInfo)
            {
                var methodNode = new MethodNode(methodInfo, null);
                List<IConnectorViewModel> inputs = new();
                List<IConnectorViewModel> outputs = new();
                NodeViewModel nodeViewModel = new() { Data = methodNode, Input = inputs, Output = outputs };
                foreach (var item in methodInfo.GetParameters().Select(a => a))
                {
                    var input = new ConnectorViewModel() { Key = item.Name, Data = methodNode.InValues.Single(a => a.Key == item.Name).Value, Flow = Enums.ConnectorFlow.Input, Node = nodeViewModel };
                    inputs.Add(input);
                }
                if (methodNode.OutValue != null)
                {
                    //var x = new ConnectorViewModel() { Flow = Enums.ConnectorFlow.Input, Node = nodeViewModel };
                    var output = new ConnectorViewModel() { Flow = Enums.ConnectorFlow.Output, Node = nodeViewModel, Key = "output", Data = methodNode.OutValue };
                    //nodeViewModel.Input.Add(x);
                    outputs.Add(output);
                }


                return nodeViewModel;
            }
            throw new Exception("sdf 322das aas");
            //return new NodeViewModel() { Data = dataContext };
        }

        public IConnectorViewModel CreatePendingConnector(object dataContext)
        {
            PendingConnectorViewModel pending = null;
            if (dataContext is bool b)
            {
                pending = new PendingConnectorViewModel() { IsInput = b };
            }
            pending = new PendingConnectorViewModel() {  };
            pending.ConnectorsChanged += a =>
            {
                if(a.Action== System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach(var item in a.NewItems)
                    {
                        if (item is PropertyInfo propertyInfo)
                        {
                            var objectConnector = new ObjectConnector(pending.Node.Data as INotifyPropertyChanged, propertyInfo);
                            //container.Resolve<IConnectorFactory>().Create(new ConnectorParameters(pending.Node.Data, propertyInfo));
                            var connectorViewModel = new ConnectorViewModel() { Data = objectConnector, Key = propertyInfo.Name, Node = pending.Node };
                            connectorViewModel.Node.Output.Add(connectorViewModel);
                        }
                    }
                }
                else if(a.Action== System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach(var propertyInfo in a.OldItems)
                    {
                        pending.Node.Output.RemoveBy(a => a.Data.Equals(propertyInfo));
                    }
                }
            };
            pending.ConnectorAdded += propertyInfo =>
            {
                var input = new ConnectorViewModel() { Data = propertyInfo, Key = propertyInfo.Name, Node = pending.Node, IsInput = true };
                pending.Node.Input.Add(input);
            };
            return pending;
        }
    }
}
