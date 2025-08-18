using DryIoc;
using MoreLinq;
using Splat;
using System.Collections.ObjectModel;
using System.Drawing;
using Utility.Changes;
using Utility.Enums;
using Utility.Infrastructure;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodify.Core;
using Utility.Nodify.Enums;
using Utility.Nodify.Models;
using Utility.Nodify.Operations;
using Utility.ServiceLocation;
using Utility.Trees;

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

    public class Shared
    {
        public static Dictionary<object, ConnectorViewModel> connectors = new();
    }

    public class NodesGenerator<T> where T : NodeViewModel, new()
    {
        ObservableCollection<T> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];

        public ObservableCollection<T> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint i = 0;
            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IResolvableNode>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item is { Value: MethodNode methodNode, Type: Utility.Changes.Type.Add })
                        {

                            var node = new T
                            {
                                Key = methodNode.Method.Name,
                                Location = settings.NodeLocationGenerator(settings, ++i),
                                Data = methodNode
                            };

                            nodes.Add(node);

                            methodNode.InValues.ForEach(a =>
                            {
                                var input = new ConnectorViewModel { Shape = ConnectorShape.Circle, Title = a.Value.Key, Data = a.Value };
                                Shared.connectors.Add(a.Value, input);
                                node.Input.Add(input);
                                input.Node = node;
                            });

                            var output = new ConnectorViewModel { Shape = ConnectorShape.Circle, Title = methodNode.Method.Name + ".", Data = methodNode.OutValue };
                            Shared.connectors.Add(methodNode.OutValue, output);
                            node.Output.Add(output);
                            output.Node = node;
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
                        var input = Shared.connectors.SingleOrDefault(a => a.Key == mConn.In).Value;
                        var output = Shared.connectors.SingleOrDefault(a => a.Key == mConn.Out).Value;
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




    public class NodesGeneratorTree<T> where T : NodeViewModel, new()
    {
        ObservableCollection<T> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];

        public ObservableCollection<T> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint indexKey = 0, i = 0;
            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IObservable<object>>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item is { Value: IObservable<object> rNode, Type: Utility.Changes.Type.Add })
                        {
                            var node = new T
                            {
                                Data = rNode,
                                Location = settings.NodeLocationGenerator(settings, ++i),
                                Key = "0." + ++indexKey
                            };

                            nodes.Add(node);

                            var input = new ConnectorViewModel { Shape = ConnectorShape.Square, Flow = ConnectorFlow.Input };
                            var output2 = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Circle, Flow = ConnectorFlow.Output };
                            Shared.connectors.Add(rNode, output2);

                            node.Output.Add(output2);
                            //Shared.connectors.Add(rNode, output);
                            node.Input.Add(input);
                            input.Node = node;
                        }
                        else if (item is { Value: IObserver<object> oNode, Type: Utility.Changes.Type.Add })
                        {
                            var node = new T
                            {
                                Data = oNode,
                                Location = settings.NodeLocationGenerator(settings, ++i),
                                Key = "0." + ++indexKey
                            };

                            nodes.Add(node);

                            var input = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Square, Flow = ConnectorFlow.Input };
                            var input2 = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Circle, Flow = ConnectorFlow.Input };
                            Shared.connectors.Add(oNode, input2);
                            node.Input.Add(input);
                            node.Input.Add(input2);
                            input.Node = node;
                        }
                    }
                });

            return nodes;
        }

        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            var root = this.root(nodes.Select(a => a.Data)) ?? throw new Exception("ccdsdsdd");
            if (nodes.Contains(root) == false)
                nodes.Add(root);

            foreach (var c in Utility.Trees.Extensions.Generic.ToTree<T, string, ConnectionViewModel>(
                nodes,
                n => idSelector(n),
                n => parentIdSelector(n),
                (a, b) => new ConnectionViewModel()
                {
                    Input = build(a, ConnectorFlow.Input, ConnectorShape.Square),
                    Output = build(b?.Output?.Node ?? root, ConnectorFlow.Output, ConnectorShape.Square),
                    Data = b
                },
                root
                ))
                connections.Add(c);

            return connections;


            IConnectorViewModel build(INodeViewModel nodeViewModel, ConnectorFlow flow, ConnectorShape shape)
            {
                if (flow == ConnectorFlow.Input)
                {

                    foreach (var x in nodeViewModel.Input.Where(a => a.Shape == shape))
                    {
                        return x;
                    }
                }
                else
                {

                    foreach (var x in nodeViewModel.Output.Where(a => a.Shape == shape))
                    {
                        return x;
                    }
                }
                throw new Exception("sd 22");

            }
        }

        private static string idSelector(T n)
        {
            if (n.Data is IGetName _name)
                return _name.Name;
            if (n.Data is IGetReference { Reference: { } reference })
                return reference is IGetName { Name: string name } ? name : throw new Exception("e33");
            throw new Exception("d3fff");

        }



        private static string? parentIdSelector(T n)
        {
            if (n.Data is IGetReference { Reference: IParent<IModel> { Parent: { } parent } })
                return parent is IGetName { Name: { } name } ? name : throw new Exception("ds32222222"); ;
            // root
            return null;

        }

        private T? root(IEnumerable<object> enumerable)
        {
            foreach (var node in enumerable)
            {
                if (node is IGetReference { Reference: { } reference } index)
                {
                    if (reference is IParent<IModel> { Parent: null } _root)
                    {
                        return (T)_root;
                    }
                    else if (reference is IParent<IModel> { Parent: { } } parent)
                    {
                        if (this.root([parent]) is T t)
                            return t;
                    }

                }
                else if (node is IModel model)
                {
                    if (model.Parent == null)
                    {
                        var _node = new T { Data = model, Key = "0" };
                        var output = new ConnectorViewModel { Node = _node, Shape = ConnectorShape.Square, Flow = ConnectorFlow.Output };
                        Shared.connectors.Add(_node, output);
                        _node.Output.Add(output);
                        output.Node = _node;
                        return _node;
                    }
                    else if (this.root([model.Parent]) is T t)
                        return t;


                }
            }
            return null;
        }
    }

    public class NodesGeneratorMaster<T> where T : NodeViewModel, new()
    {
        private T x;
        private T y;
        private ConnectorViewModel input;
        private ConnectorViewModel output;

        public ObservableCollection<T> GenerateNodes(NodesGeneratorSettings settings)
        {
            x = new T() { Data = new Utility.Nodify.ViewModels.DiagramViewModel(Locator.Current.GetService<IContainer>()) { Key = "Tree", Arrangement = Utility.Enums.Arrangement.Tree } };
          
            
            y = new T() { Data = new Utility.Nodify.ViewModels.DiagramViewModel(Locator.Current.GetService<IContainer>()) { Key = "Diagram", Arrangement = Utility.Enums.Arrangement.Standard } };
            return new ObservableCollection<T>([x, y]);
        }

        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            ObservableCollection<ConnectionViewModel> connections = [];

            (Utility.Globals.Resolver.Resolve<IServiceResolver>() as IObservable<Set<IResolvableConnection>>)
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item.Value is MethodConnection { In: { } @in, Out: { } @out } mConn && item.Type == Utility.Changes.Type.Add)
                        {
                            if (@out is MethodConnector m && @in is MethodConnector mIn)
                            {
                                continue;
                            }
                            var input = Shared.connectors.SingleOrDefault(a => a.Key == @in).Value;
                            var output = Shared.connectors.SingleOrDefault(a => a.Key == @out).Value;
                            var connection = new ConnectionViewModel
                            {
                                Input = input,
                                Output = output,
                                Data = mConn
                            };

                            //connections.Add(connection);
                        }
                        else
                            throw new Exception("E$$3");
                    }
                });

            output = new ConnectorViewModel() { Node =x, Shape = ConnectorShape.Triangle, Flow = ConnectorFlow.Output };
            x.Output.Add(output);
            output.Node = x;

            input = new ConnectorViewModel() { Node = y, Shape = ConnectorShape.Triangle, Flow = ConnectorFlow.Input };
            y.Input.Add(input);
            input.Node = y;

            var connection = new ConnectionViewModel
            {

     
                    Input = input,
                    Output = output
                
            };

            connections.Add(connection);
            return connections;
        }

    }
}
