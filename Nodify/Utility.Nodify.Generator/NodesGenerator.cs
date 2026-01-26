using System.Collections.ObjectModel;
using Utility.Changes;
using Utility.Enums;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.Nodify.Base.Abstractions;
using Utility.Reactives;
using Utility.ServiceLocation;

namespace Nodify.Playground
{

    public class Shared
    {
        public static Dictionary<object, ConnectorViewModel> inputConnectors = new();
        public static Dictionary<object, ConnectorViewModel> outputConnectors = new();
    }

    public class NodesGenerator
    {
        ObservableCollection<NodeViewModel> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];

        public ObservableCollection<NodeViewModel> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint i = 0;
            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IResolvableNode>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item is { Value: MethodNode methodNode, Type: Utility.Changes.Type.Add })
                        {
                            var node = new NodeViewModel
                            {
                                Key = methodNode.MethodInfo.Name,
                                Location = settings.NodeLocationGenerator(settings, ++i),
                                Data = methodNode,
                            };

                            nodes.Add(node);

                            methodNode.InValues.ForEach(a =>
                            {
                                var input = new ConnectorViewModel { Shape = FlatShape.Circle, Key = a.Value.Key, Data = a.Value };
                                Shared.inputConnectors.Add(a.Value, input);
                                node.Inputs.Add(input);
                                input.Node = node;
                            });

                            if (methodNode.OutValue is { })
                            {
                                var output = new ConnectorViewModel { Shape = FlatShape.Circle, Key = methodNode.MethodInfo.Name + ".", Data = methodNode.OutValue };
                                Shared.outputConnectors.Add(methodNode.OutValue, output);
                                node.Outputs.Add(output);
                                output.Node = node;
                            }
                        }
                        else
                        {

                        }
                    }
                });

            uint indexKey = 0;
            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IObservable<object>>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item is { Value: IObservable<object> connector, Type: Utility.Changes.Type.Add })
                        {
                            string _name = null;
                            if (connector is IGetReference
                                { Reference: IGetName { Name: { } name } }
                            and
                                {
                                    Reference: IGetData { Data: { } data }
                            and INodeViewModel reference
                                })
                            {
                                _name = name;
                            }
                            else
                            {
                                throw new Exception("DSF3d54645dfd");
                            }

                            var output = new ConnectorViewModel { Node = reference, Shape = FlatShape.Circle, Flow = IO.Output, Key = "output", Data = connector };
                            Shared.outputConnectors.Add(connector, output);
                            reference.Outputs.Add(output);
                            reference.Outputs.AndChanges<object>().Subscribe(a =>
                            {

                            });
                        }
                    }
                });

            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IObserver<object>>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item is { Value: IObserver<object> connector, Type: Utility.Changes.Type.Add })
                        {
                            string _name = null;
                            if (connector is IGetReference
                                { Reference: IGetName { Name: { } name } }
                            and
                                {
                                    Reference: IGetData { Data: { } data }
                            and INodeViewModel reference
                                })
                            {
                                _name = name;
                            }
                            else
                            {
                                throw new Exception("DSF3d54645dfd");
                            }

                            var input = new ConnectorViewModel { Shape = FlatShape.Square, Flow = IO.Input, Node = reference, Key = "input", Data = connector };
                            Shared.inputConnectors.Add(connector, input);
                            reference.Inputs.Add(input);
                        }
                    }
                });

            return nodes;
        }

        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            HashSet<NodeViewModel> visited = new HashSet<NodeViewModel>(nodes.Count);
            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IResolvableConnection>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item.Value is MethodConnection { In: { }, Out: { } } mConn && item.Type == Utility.Changes.Type.Add)
                        {
                            ObservableCollection<IConnectorViewModel> inConnectors = new();
                            foreach (var x in mConn.In)
                            {
                                var input = Shared.inputConnectors.SingleOrDefault(a => a.Key == x).Value;
                                if (input != null)
                                    inConnectors.Add(input);
                                else
                                {
                                    throw new Exception("DSV3ddd");
                                }
                            }
                            var output = Shared.outputConnectors.SingleOrDefault(a => a.Key == mConn.Out).Value;
                            if (output == null)
                            {
                                throw new Exception("sdsf32vdfd");
                            }
                            var connection = new ConnectionViewModel
                            {
                                Inputs = inConnectors,
                                Output = output,
                                Data = mConn,
                                Key = Guid.NewGuid().ToString()
                            };

                            connections.Add(connection);
                        }
                        else
                        {

                        }
                        //throw new Exception("E$$3");
                    }
                });

            return connections;
        }
    }
}