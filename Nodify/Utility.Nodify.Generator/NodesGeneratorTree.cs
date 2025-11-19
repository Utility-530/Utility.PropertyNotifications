using DryIoc;
using System.Collections.ObjectModel;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Interfaces;
using Utility.Trees.Abstractions;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Enums;

namespace Nodify.Playground
{
    public class NodesGeneratorTree
    {
        ObservableCollection<NodeViewModel> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];

        public ObservableCollection<NodeViewModel> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint i = 0;
            (Utility.Globals.Resolver.Resolve<IModelResolver>())
                .Subscribe(item =>
                {
                    var index = Utility.Trees.Extensions.Generic.Index(item, item => item.Parent(), (item, child) => Utility.Helpers.NonGeneric.Linq.IndexOf((item as IYieldItems).Items(), a => (a as IGetName).Name.Equals((child as IGetName).Name)));

                    var node = new NodeViewModel
                    {
                        Data = item,
                        Location = settings.NodeLocationGenerator(settings, ++i),
                        Key = new Utility.Structs.Index(index).ToString(),
                        Input = [],
                        Output = []
                    };

                    nodes.Add(node);

                    var input = new ConnectorViewModel { Shape = FlatShape.Square, Flow = IO.Input, Key = "input", Node = node, Data = null };
                    var output1 = new ConnectorViewModel { Node = node, Shape = FlatShape.Circle, Flow = IO.Output , Key = "ouput1", Data = null };
                    var output2 = new ConnectorViewModel { Node = node, Shape = FlatShape.Square, Flow = IO.Output, Key = "ouput2", Data = null };
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

        NodeViewModel? root = null;
        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            bool flag = false;
            nodes.AndAdditions<NodeViewModel>().Subscribe(node =>
            {
                this.root ??= nodes.SingleOrDefault(a => a.Key == "0");

                //var index = nodes.IndexOf((T)node);
                if (root != null && !flag)
                {
                    Task.Run(() =>
                    {
                        flag = true;
                        Utility.Trees.Extensions.Generic.ToTree<ObservableCollection<NodeViewModel>, NodeViewModel, string, ConnectionViewModel>(
                        nodes,
                        n => idSelector(n),
                        n => parentIdSelector(n),
                        (a, b) =>
                        {
                            try
                            {
                                return new ConnectionViewModel()
                                {
                                    Input = build(b?.Output?.Node ?? root, IO.Output, FlatShape.Square),
                                    Output =build(a, IO.Input, FlatShape.Square) ,
                                    Data = b
                                };
                            }
                            catch (Exception ex)
                            {
                                return null;
                            }
                        },
                        this.root
                        ).Subscribe(c =>
                        {
                            if (c != null)
                                if (connections.ToArray().Any(a => a?.Input == c.Input && a?.Output == c.Output) == false)
                                    connections.Add(c);
                        });
                    });


                }
            });

            return connections;


            IConnectorViewModel build(INodeViewModel nodeViewModel, IO flow, FlatShape shape)
            {
                if (flow == IO.Input)
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

        private static string idSelector(NodeViewModel n)
        {
            if (n.Data is IGetName _name)
                return _name.Name;
            throw new Exception("d3fff");
        }

        private static string? parentIdSelector(NodeViewModel n)
        {
            if (n.Data is IGetParent<IReadOnlyTree> { Parent: { } parent })
                return parent is IGetName { Name: { } name } ? name : throw new Exception("ds32222222"); ;
            // root
            return null;
        }

        private NodeViewModel? _root(IEnumerable<object> enumerable)
        {
            foreach (var node in enumerable)
            {
                if (node is IReadOnlyTree model)
                {
                    if (model.Parent() == null)
                    {
                        var _node = new NodeViewModel { Data = model, Key = "0", Input = [], Output = [] };
                        var output = new ConnectorViewModel { Node = _node, Shape = FlatShape.Square, Flow = IO.Output, Key = "output", Data = null };
                        Shared.serviceConnectors.Add(_node, output);
                        _node.Output.Add(output);
                        output.Node = _node;
                        return _node;
                    }
                    else if (this._root([model.Parent()]) is NodeViewModel t)
                        return t;
                }
            }
            return null;
        }
    }
}