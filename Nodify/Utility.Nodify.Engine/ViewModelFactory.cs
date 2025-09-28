using DryIoc;
using LanguageExt.Pipes;
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

namespace Utility.Nodify.Engine
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly DryIoc.IContainer container;

        public ViewModelFactory(DryIoc.IContainer container)
        {
            this.container = container;
        }

        public IConnectionViewModel CreateConnection(IConnectorViewModel source, IConnectorViewModel target)
        {


            var connectionViewModel = new ConnectionViewModel
            {
                Input = target,
                Output = source,
                //Data = container.Resolve<IConnectionFactory>().CreateConnection(source.Data, target.Data),
                IsDirectionForward = source.Node is IGetData { Data : Tree } && target.Node is IGetData { Data: { } data } && data.GetType().Name.Contains("observable", StringComparison.InvariantCultureIgnoreCase) == true
            };

            return connectionViewModel;
        }


        public bool CanCreateConnection(IConnectorViewModel source, IConnectorViewModel? target)
            => target == null || source != target && source.Node != target.Node && source.IsInput != target.IsInput;


        public IConnectorViewModel CreateConnector(object dataContext)
        {
            if (dataContext is ConnectorParameters { Guid: Guid guid, Data: var data, IsInput: bool isInput, Key: var key, Node: var node } s)
            {
                return new ConnectorViewModel()
                {
                    Guid = guid,
                    IsInput = isInput,
                    Key = "ObjectProp",
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
                return new NodeViewModel() {  Data = instanceKey.Data, Diagram = container.Resolve<IDiagramViewModel>(), Input = [], Output = [] };
            }
            else if (dataContext is IReadOnlyTree tree)
            {
                if (tree is NodeViewModel nodeViewModel)
                {
                    var x = CreatePendingConnector(true);// new PendingConnectorViewModel() { IsInput = true };
                    var y = CreatePendingConnector(null);

                    nodeViewModel.Input = new CollectionWithFixedLast<IConnectorViewModel>(x);
                    nodeViewModel.Output = new CollectionWithFixedLast<IConnectorViewModel>(y);

                    x.Node = nodeViewModel;
                    y.Node = nodeViewModel;
                    return nodeViewModel;
                }
            }
            else if (dataContext is MethodInfo methodInfo)
            {
                List<IConnectorViewModel> inputs = new();
                List<IConnectorViewModel> outputs = new();
                NodeViewModel nodeViewModel = new() { Data = methodInfo, Input = inputs, Output = outputs, Key = new GuidKey(Guid.NewGuid()) };
                foreach (var parameter in methodInfo.GetParameters())
                {
                    var input = new ConnectorViewModel() { Key = parameter.Name, Data = parameter, Flow = IO.Input, Node = nodeViewModel };
                    inputs.Add(input);
                }
                if (methodInfo.ReturnParameter != null)
                {
                    var output = new ConnectorViewModel() { Flow = IO.Output, Node = nodeViewModel, Key = "output", Data = methodInfo.ReturnParameter };
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
                            var connectorViewModel = new ConnectorViewModel() { Data = propertyInfo, Key = propertyInfo.Name, Node = pending.Node };
                            connectorViewModel.Node.Output.Add(connectorViewModel);
                        }
                    }
                }
                else if(a.Action== System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach(var propertyInfo in a.OldItems)
                    {
                        pending.Node.Output.RemoveBy(a => a is IGetData { Data: { } data } && data.Equals(propertyInfo));
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
