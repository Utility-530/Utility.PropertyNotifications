using MoreLinq;
using Splat;
using System.Collections.ObjectModel;
using System.Drawing;
using Utility.Changes;
using Utility.Models;
using Utility.Nodify.Enums;
using Utility.Nodify.Models;
using Utility.Nodify.Operations;
using Utility.Services;

namespace Nodify.Playground
{
    public struct NodesGeneratorSettings
    {
        private static readonly Random _rand = new Random();

        public NodesGeneratorSettings(uint count)
        {
            GridSnap = 15;
            MinNodesCount = MaxNodesCount = count;
            MinInputCount = MinOutputCount = 0;
            MaxInputCount = MaxOutputCount = 7;

            ConnectorNameGenerator = (s, i) => $"{new string('C', (int)i % 5)} {i}";
            NodeNameGenerator = (s, i) => $"Node {i}";
            NodeLocationGenerator = (s, i) =>
            {
                static double EaseOut(double percent, double increment, double start, double end, double total)
                    => -end * (increment /= total) * (increment - 2) + start;

                var xDistanceBetweenNodes = _rand.Next(150, 350);
                var yDistanceBetweenNodes = _rand.Next(200, 350);
                var randSignX = _rand.Next(0, 100) > 50 ? 1 : -1;
                var randSignY = _rand.Next(0, 100) > 50 ? 1 : -1;
                var gridOffsetX = i * xDistanceBetweenNodes;
                var gridOffsetY = i * yDistanceBetweenNodes;

                var x = gridOffsetX * Math.Sin(xDistanceBetweenNodes * randSignX / (i + 1));
                var y = gridOffsetY * Math.Sin(yDistanceBetweenNodes * randSignY / (i + 1));
                var easeX = x * EaseOut(i / count, i, 1, 0.01, count);
                var easeY = y * EaseOut(i / count, i, 1, 0.01, count);

                x = s.Snap((int)easeX);
                y = s.Snap((int)easeY);

                return new PointF((float)x, (float)y);
            };
        }

        public uint GridSnap;
        public uint MinNodesCount;
        public uint MaxNodesCount;
        public uint MinInputCount;
        public uint MaxInputCount;
        public uint MinOutputCount;
        public uint MaxOutputCount;

        public Func<NodesGeneratorSettings, uint, string?> ConnectorNameGenerator;
        public Func<NodesGeneratorSettings, uint, string?> NodeNameGenerator;
        public Func<NodesGeneratorSettings, uint, PointF> NodeLocationGenerator;

        public int Snap(int x)
            => x / (int)GridSnap * (int)GridSnap;
    }

    public class NodesGenerator<T> where T : NodeViewModel, new()
    {
        ObservableCollection<T> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];
        Dictionary<object, ConnectorViewModel> connectors = new();
        public ObservableCollection<T> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint i = 0;
            (Locator.Current.GetService<ServiceResolver>() as IObservable<Set<IResolvableNode>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item is { Value: MethodNode methodNode, Type: Utility.Changes.Type.Add })
                        {

                            var node = new T
                            {
                                Title = methodNode.Method.Name,
                                Location = settings.NodeLocationGenerator(settings, ++i),
                                Core = new MethodOperation(methodNode.Method.MethodInfo)
                            };

                            nodes.Add(node);

                            methodNode.InValues.ForEach(a =>
                            {
                                var input = new ConnectorViewModel { Shape = ConnectorShape.Circle, Title = a.Value.Key, Value = a.Value.Value };
                                connectors.Add(a.Value, input);
                                node.Input.Add(input);
                                input.Node = node;
                            });

                            var output = new ConnectorViewModel { Shape = ConnectorShape.Circle, Title = methodNode.Method.Name + "." };
                            connectors.Add(methodNode.OutValue, output);
                            node.Output.Add(output);
                            output.Node = node;

                            //node.Input.AddRange(GenerateConnectors(settings, _rand.Next((int)settings.MinInputCount, (int)settings.MaxInputCount + 1)));
                            //node.Output.AddRange(GenerateConnectors(settings, _rand.Next((int)settings.MinOutputCount, (int)settings.MaxOutputCount + 1)));

                        }
                        else if (item is { Value: ObservableNode rNode, Type: Utility.Changes.Type.Add })
                        {
                            var node = new T
                            {
                                Title = rNode.Name,
                                Location = settings.NodeLocationGenerator(settings, ++i)

                            };

                            nodes.Add(node);

                            //rNode.InValues.ForEach(a =>
                            //{
                            //    var input = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Circle };
                            //    connectors.Add(a.Value, input);
                            //    node.Input.Add(input);
                            //});

                            var output = new ConnectorViewModel { Shape = ConnectorShape.Circle };
                            connectors.Add(rNode.Connector, output);
                            node.Output.Add(output);
                            output.Node = node;
                        }
                        else if (item is { Value: ObserverNode oNode, Type: Utility.Changes.Type.Add })
                        {
                            var node = new T
                            {
                                Title = oNode.Name,
                                Location = settings.NodeLocationGenerator(settings, ++i)
                            };

                            nodes.Add(node);

                            //rNode.InValues.ForEach(a =>
                            //{
                            //    var input = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Circle };
                            //    connectors.Add(a.Value, input);
                            //    node.Input.Add(input);
                            //});

                            var input = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Circle };
                            connectors.Add(oNode.Connector, input);
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
            (Locator.Current.GetService<ServiceResolver>() as IObservable<Set<IResolvableConnection>>).Subscribe(set =>
            {
                foreach (var item in set)
                {
                    if (item.Value is MethodConnection mConn && item.Type == Utility.Changes.Type.Add)
                    {
                        var input = connectors.SingleOrDefault(a => a.Key == mConn.In).Value;
                        var output = connectors.SingleOrDefault(a => a.Key == mConn.Out).Value;
                        var connection = new ConnectionViewModel
                        {
                            Input = input,
                            Output = output
                        };


                        T n1 = null, n2 = null;


                        if (n1 is not null && n2 is not null)
                        {
                            if (n1 == n2 && !(visited.Add(n1) && visited.Add(n2)))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            //continue;
                        }
                        connections.Add(connection);
                    }
                    else
                        throw new Exception("E$$3");
                }
            });



            return connections;
        }
    }
}
