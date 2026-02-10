using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Models.Diagrams;
using Utility.Nodify.Base;
using Utility.Nodify.Base.Abstractions;
using Utility.Reactives;
using Utility.Trees;
using Utility.Trees.Abstractions;
using Utility.Helpers.Generic;
using Utility.Nodes;
using Utility.Keys;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.ServiceLocation;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Engine
{
    public class ViewModelFactory : IViewModelFactory
    {


        public ViewModelFactory()
        {
        }

        public IConnectionViewModel CreateConnection(IConnectorViewModel source, IConnectorViewModel target)
        {


            var connectionViewModel = new ConnectionViewModel
            {
                Input = target,
                Output = source,
                //Data = container.Resolve<IConnectionFactory>().CreateConnection(source.Data, target.Data),
                IsDirectionForward = source.Node is IGetData { Data: Tree } && target.Node is IGetData { Data: { } data } && data.GetType().Name.Contains("observable", StringComparison.InvariantCultureIgnoreCase) == true
            };

            return connectionViewModel;
        }


        public bool CanCreateConnection(IConnectorViewModel source, IConnectorViewModel? target)
            => target == null || source != target && source.Node != target.Node && source.IsInput != target.IsInput;


        public IConnectorViewModel CreateConnector(object dataContext)
        {
            var connector = create();
            connector
                .WhenReceivedFrom(a => a.IsExpanded)
                .Subscribe(isExpanded =>
                {
                    Globals.Resolver.Resolve<IDiagramViewModel>().DisablePanning = isExpanded;
                });
            return connector;

            ConnectorViewModel create()
            {
                if (dataContext is ConnectorParameters { Guid: Guid guid, Data: var data, IsInput: bool isInput, Key: var key, Node: var node } s)
                {
                    return new ConnectorViewModel()
                    {
                        Guid = guid,
                        IsInput = isInput,
                        Name = "ObjectProp",
                        Node = node,
                        Data = data,
                        IsExpanded =false,
                    };
                }
                else if (dataContext is string _s)
                {
                    return new ConnectorViewModel() { Data = dataContext, Name = _s, IsExpanded = false };
                }

                return new ConnectorViewModel() { Data = dataContext, Name = "", IsExpanded = false, };
            }
        }

        public INodeViewModel CreateNode(object dataContext)
        {
            if (dataContext is InstanceKey instanceKey)
            {
                return new NodeViewModel() { Data = instanceKey.Data, Diagram = Utility.Globals.Resolver.Resolve<IDiagramViewModel>() };
            }
            else if (dataContext is IReadOnlyTree tree)
            {
                if (tree is NodeViewModel nodeViewModel)
                {
                    var x = CreatePendingConnector(true);
                    var y = CreatePendingConnector(null);

                    nodeViewModel.Inputs = new CollectionWithFixedLast<IConnectorViewModel>(x);
                    nodeViewModel.Outputs = new CollectionWithFixedLast<IConnectorViewModel>(y);

                    x.Node = nodeViewModel;
                    y.Node = nodeViewModel;
                    return nodeViewModel;
                }
            }
            else if (dataContext is MethodInfo methodInfo)
            {
                List<IConnectorViewModel> inputs = new();
                List<IConnectorViewModel> outputs = new();
                NodeViewModel nodeViewModel = new() { Data = methodInfo, Inputs = inputs, Outputs = outputs, Key = new GuidKey(Guid.NewGuid()) };
                foreach (var parameter in methodInfo.GetParameters())
                {
                    var input = new ConnectorViewModel() { Name = parameter.Name, IsInput = true, Guid = Guid.NewGuid(), Data = parameter, Flow = IO.Input, Node = nodeViewModel };
                    inputs.Add(input);
                }
                if (methodInfo.ReturnParameter != null)
                {
                    var output = new ConnectorViewModel() { Flow = IO.Output, Guid = Guid.NewGuid(), Node = nodeViewModel, Name = "output", Data = methodInfo.ReturnParameter };
                    outputs.Add(output);
                }

                return nodeViewModel;
            }
            throw new Exception("sdf 322das aas");
        }

        public IConnectorViewModel CreatePendingConnector(object dataContext)
        {
            PendingConnectorViewModel pending = null;
            if (dataContext is bool b)
            {
                pending = new PendingConnectorViewModel() { IsInput = b, IsExpanded =false };
            }
            pending = new PendingConnectorViewModel() { IsExpanded = false };

            pending.WhenReceivedFrom(a => a.IsExpanded).Subscribe(a =>
            {
                Globals.Resolver.Resolve<IDiagramViewModel>().DisablePanning = a;
            });
            pending.ConnectorsChanged += a =>
            {
                if (a.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in a.NewItems)
                    {
                        if (item is IGetData { Data: PropertyInfo propertyInfo })
                        {
                            var parameters = new ConnectorParameters(Guid.NewGuid(), false, propertyInfo.Name, pending.Node, propertyInfo);
                            var connectorViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateConnector(parameters);
                            connectorViewModel.Node.Outputs.Add(connectorViewModel);
                        }
                        if (item is PropertyInfo _propertyInfo )
                        {
                            var parameters = new ConnectorParameters(Guid.NewGuid(), false, _propertyInfo.Name, pending.Node, _propertyInfo);
                            var connectorViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateConnector(parameters);
                            connectorViewModel.Node.Outputs.Add(connectorViewModel);
                        }
                    }
                }
                else if (a.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach (var propertyInfo in a.OldItems)
                    {
                        pending.Node.Outputs.RemoveBy(a => a is IGetData { Data: { } data } && data.Equals(propertyInfo));
                    }
                }
            };

            pending.ConnectorAdded += propertyInfo =>
            {     
                var parameters = new ConnectorParameters(Guid.NewGuid(), true, propertyInfo.Name, pending.Node, propertyInfo);
                var connectorViewModel = Globals.Resolver.Resolve<IViewModelFactory>().CreateConnector(parameters);
                pending.Node.Inputs.Add(connectorViewModel);
            };
            return pending;
        }
    }
}
