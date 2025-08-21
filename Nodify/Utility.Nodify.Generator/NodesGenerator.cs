using DryIoc;
using System.Collections.ObjectModel;
using System.Drawing;
using Utility.Changes;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Diagrams;
using Utility.Nodify.Core;
using Utility.Nodify.Enums;
using Utility.Nodify.Models;
using Utility.ServiceLocation;

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
        public static Dictionary<object, ConnectorViewModel> serviceConnectors = new();

        public static Dictionary<object, ConnectorViewModel> modelConnectors = new();
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
                                Shared.serviceConnectors.Add(a.Value, input);
                                node.Input.Add(input);
                                input.Node = node;
                            });

                            if(methodNode.OutValue is { } )
                            {
                                var output = new ConnectorViewModel { Shape = ConnectorShape.Circle, Title = methodNode.Method.Name + ".", Data = methodNode.OutValue };
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
                            var node = new T
                            {
                                Data = rNode,
                                //Location = settings.NodeLocationGenerator(settings, ++i),
                                Key = _name ?? (++indexKey).ToString(),
                            };

                            nodes.Add(node);

                            var input = new ConnectorViewModel { Shape = ConnectorShape.Square, Flow = ConnectorFlow.Input };
                            var output2 = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Circle, Flow = ConnectorFlow.Output };
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




    public class NodesGeneratorTree<T> where T : NodeViewModel, new()
    {
        ObservableCollection<T> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];

        public ObservableCollection<T> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint i = 0;
            (Utility.Globals.Resolver.Resolve<IModelResolver>())
                .Subscribe(item =>
                {
                    var index = Utility.Trees.Extensions.Generic.Index(item, item => (item as IGetParent<IModel>).Parent, (item, child) => Utility.Helpers.NonGeneric.Linq.IndexOf((item as IYieldChildren).Children, a => a == child));

                    var node = new T
                    {
                        Data = item,
                        Location = settings.NodeLocationGenerator(settings, ++i),
                        Key = new Utility.Structs.Index(index).ToString()
                    };

                    nodes.Add(node);

                    var input = new ConnectorViewModel { Shape = ConnectorShape.Square, Flow = ConnectorFlow.Input };
                    var output1 = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Circle, Flow = ConnectorFlow.Output };
                    var output2 = new ConnectorViewModel { Node = node, Shape = ConnectorShape.Square, Flow = ConnectorFlow.Output };
                    Shared.serviceConnectors.Add(item, output1);
                    Shared.modelConnectors.Add(item, output2);

                    node.Output.Add(output1);
                    node.Output.Add(output2);
                    //Shared.connectors.Add(rNode, output);
                    node.Input.Add(input);
                    input.Node = node;


                });

            return nodes;
        }

        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            var root = nodes.Single(a => a.Key == "0") ?? throw new Exception("ccdsdsdd");

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
            throw new Exception("d3fff");
        }

        private static string? parentIdSelector(T n)
        {
            if (n.Data is IGetParent<IModel> { Parent: { } parent })
                return parent is IGetName { Name: { } name } ? name : throw new Exception("ds32222222"); ;
            // root
            return null;
        }

        private T? root(IEnumerable<object> enumerable)
        {
            foreach (var node in enumerable)
            {
                if (node is IModel model)
                {
                    if ((model as IGetParent<IModel>).Parent == null)
                    {
                        var _node = new T { Data = model, Key = "0" };
                        var output = new ConnectorViewModel { Node = _node, Shape = ConnectorShape.Square, Flow = ConnectorFlow.Output };
                        Shared.serviceConnectors.Add(_node, output);
                        _node.Output.Add(output);
                        output.Node = _node;
                        return _node;
                    }
                    else if (this.root([(model as IGetParent<IModel>).Parent]) is T t)
                        return t;
                }
            }
            return null;
        }
    }

    public class NodesGeneratorMaster<T> where T : NodeViewModel, new()
    {
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
                            var input = Shared.serviceConnectors.SingleOrDefault(a => a.Key == @in).Value;
                            var output = Shared.serviceConnectors.SingleOrDefault(a => a.Key == @out).Value;
                            var connection = new ConnectionViewModel
                            {
                                Input = input,
                                Output = output,
                                Data = mConn
                            };

                            connections.Add(connection);
                        }
                        else
                            throw new Exception("E$$3");
                    }
                });

            return connections;
        }

    }




    public class ReactiveNodeGroupManager
    {
        private readonly Dictionary<string, ConnectionViewModel> _nodes = new Dictionary<string, ConnectionViewModel>();
        private List<List<NodeViewModel>> _currentGroups = new List<List<NodeViewModel>>();
        NodeGroupHelper.UnionFind unionFind = new();
        public event Action<List<List<NodeViewModel>>> GroupsChanged;

        public void AddNode(ConnectionViewModel node)
        {
            _nodes[node.Key] = node;
            RecalculateGroups();
        }

        public void AddNodes(IEnumerable<ConnectionViewModel> nodes)
        {
            foreach (var node in nodes)
            {
                _nodes[node.Key] = node;
            }
            RecalculateGroups();
        }

        public void RemoveNode(string nodeKey)
        {
            if (_nodes.Remove(nodeKey))
            {
                RecalculateGroups();
            }
        }

        private void RecalculateGroups()
        {
            _currentGroups = NodeGroupHelper.GroupConnectedNodesWithUnionFind(_nodes.Values, unionFind);
            GroupsChanged?.Invoke(_currentGroups);
        }

        public List<List<NodeViewModel>> GetCurrentGroups() => _currentGroups;
    }


    public static class NodeGroupHelper
    {
        public class UnionFind
        {
            private Dictionary<string, string> parent = new Dictionary<string, string>();
            private Dictionary<string, int> rank = new Dictionary<string, int>();

            public void MakeSet(string x)
            {
                if (!parent.ContainsKey(x))
                {
                    parent[x] = x;
                    rank[x] = 0;
                }
            }

            public string Find(string x)
            {
                if (!parent.ContainsKey(x))
                    MakeSet(x);

                if (parent[x] != x)
                    parent[x] = Find(parent[x]);
                return parent[x];
            }

            public void Union(string x, string y)
            {
                string rootX = Find(x);
                string rootY = Find(y);

                if (rootX != rootY)
                {
                    if (rank[rootX] < rank[rootY])
                        parent[rootX] = rootY;
                    else if (rank[rootX] > rank[rootY])
                        parent[rootY] = rootX;
                    else
                    {
                        parent[rootY] = rootX;
                        rank[rootX]++;
                    }
                }
            }

            public Dictionary<string, List<string>> GetGroups()
            {
                return parent.Keys
                    .GroupBy(Find)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
        }

        public static List<List<NodeViewModel>> GroupConnectedNodesWithUnionFind(
            IEnumerable<ConnectionViewModel> connections, UnionFind existingUnionFind)
        {

            var uf = existingUnionFind;

            // Initialize all nodes
            //foreach (var node in nodeList)
            //{
            //    uf.MakeSet(node.Key);
            //}
            List<NodeViewModel> nodeList = new();
            // Union connected nodes
            //foreach (var node in nodeList)
            //{
            //    var connections = node.Output.SelectMany(a => a.Connections.Select(c => c.Input.Node.Key));
            foreach (var connection in connections)
            {
                if (connection.Input.Node is NodeViewModel node && connection.Output.Node is NodeViewModel _node)
                {
                    uf.MakeSet(node.Key);
                    uf.MakeSet(_node.Key);
                    uf.Union(node.Key, _node.Key);
                    nodeList.Add(node);
                    nodeList.Add(_node);
                }
            }
            //}

            // Create groups and assign GroupKey
            var groupDict = uf.GetGroups();
            var groups = new List<List<NodeViewModel>>();
            int groupIndex = 0;

            foreach (var group in groupDict.Values)
            {
                var groupKey = groupIndex++.ToString();
                var nodeGroup = new List<NodeViewModel>();

                foreach (var nodeKey in group)
                {
                    var node = nodeList.First(n => n.Key == nodeKey);
                    node.GroupKey = groupKey;
                    nodeGroup.Add(node);
                }
                groups.Add(nodeGroup);
            }

            return groups;
        }
    }
}