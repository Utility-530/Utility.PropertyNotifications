using DryIoc;
using System.Collections.ObjectModel;
using Utility.Changes;
using Utility.Enums;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Diagrams;
using Utility.Nodes;
using Utility.ServiceLocation;

namespace Nodify.Playground
{

    public class Shared
    {
        public static Dictionary<object, ConnectorViewModel> serviceConnectors = new();

        public static Dictionary<object, ConnectorViewModel> modelConnectors = new();
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
                                Input = [],
                                Output = []
                            };

                            nodes.Add(node);

                            methodNode.InValues.ForEach(a =>
                            {
                                var input = new ConnectorViewModel { Shape = FlatShape.Circle, Key = a.Value.Key, Data = a.Value };
                                Shared.serviceConnectors.Add(a.Value, input);
                                node.Input.Add(input);
                                input.Node = node;
                            });

                            if (methodNode.OutValue is { })
                            {
                                var output = new ConnectorViewModel { Shape = FlatShape.Circle, Key = methodNode.MethodInfo.Name + ".", Data = methodNode.OutValue };
                                Shared.serviceConnectors.Add(methodNode.OutValue, output);
                                node.Output.Add(output);
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
                        if (item is { Value: IObservable<object> rNode, Type: Utility.Changes.Type.Add })
                        {
                            string _name = null;
                            if (rNode is IGetReference { Reference: IGetName { Name: { } name } })
                            {
                                _name = name;
                            }
                            var node = new NodeViewModel
                            {
                                Data = rNode,
                                //Location = settings.NodeLocationGenerator(settings, ++i),
                                Key = _name ?? (++indexKey).ToString(),
                                Input = [],
                                Output = []
                            };

                            nodes.Add(node);

                            var input = new ConnectorViewModel { Shape = FlatShape.Square, Flow = IO.Input, Node = node, Key = "input", Data = null };
                            var output2 = new ConnectorViewModel { Node = node, Shape = FlatShape.Circle, Flow = IO.Output, Key = "output", Data = null };
                            Shared.serviceConnectors.Add(rNode, output2);

                            node.Output.Add(output2);
                            //Shared.connectors.Add(rNode, output);
                            node.Input.Add(input);
                            input.Node = node;
                        }
                    }
                });

            return nodes;
        }

        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            HashSet<NodeViewModel> visited = new HashSet<NodeViewModel>(nodes.Count);
            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IResolvableConnection>>).Subscribe(set =>
            {
                foreach (var item in set)
                {
                    if (item.Value is MethodConnection { In: MethodConnector, Out: MethodConnector } mConn && item.Type == Utility.Changes.Type.Add)
                    {
                        var input = Shared.serviceConnectors.SingleOrDefault(a => a.Key == mConn.In).Value;
                        var output = Shared.serviceConnectors.SingleOrDefault(a => a.Key == mConn.Out).Value;
                        var connection = new ConnectionViewModel
                        {
                            Input = input,
                            Output = output,
                            Data = mConn
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