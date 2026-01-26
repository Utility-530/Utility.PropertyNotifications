using System.Collections.ObjectModel;
using Utility.Enums;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Nodify.Base;
using Utility.Nodify.Base.Abstractions;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions.Async;

namespace Nodify.Playground
{
    public class NodesGeneratorTree
    {
        ObservableCollection<NodeViewModel> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];

        public ObservableCollection<NodeViewModel> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint i = 0;
            (Utility.Globals.Resolver.Resolve<INodeRoot>())
                .Subscribe(root =>
                {
                    root
                    .SelfAndDescendants()
                    .Subscribe(item =>
                    {
                        if (item is not NodeViewModel node)
                        {
                            var index = Utility.Trees.Extensions.Generic.Index<IReadOnlyTree>(item, item => item.Parent(), (item, child) => Utility.Helpers.NonGeneric.Linq.IndexOf((item as IYieldItems).Items(), a => (a as IGetName).Name.Equals((child as IGetName).Name)));

                            node = new NodeViewModel
                            {
                                Data = item,
                                Location = settings.NodeLocationGenerator(settings, ++i),
                                Key = new Utility.Structs.Index(index).ToString(),
                            };                          
                        }
                        else
                        {
                            node.Location = settings.NodeLocationGenerator(settings, ++i);
                        }

                        if (node.Parent() is not null)
                        {
                            var input = new ConnectorViewModel { Node = node, Shape = FlatShape.Square, Flow = IO.Input, Key = "input" };
                            node.Inputs.Add(input);
                        }
                        var output = new ConnectorViewModel { Node = node, Shape = FlatShape.Square, Flow = IO.Output, Key = "ouput2" };
                        node.Outputs.Add(output);
                        nodes.Add(node);
                    });
                });

            return nodes;
        }

        public ObservableCollection<ConnectionViewModel> GenerateConnections()
        {
            ObservableCollection<ConnectionViewModel> collection = new();
            bool flag = false;
            nodes.AndAdditions<NodeViewModel>().Subscribe(node =>
            {

                try
                {
                    if (node.Parent() is INodeViewModel nodeViewModel)
                    {
                        var x = new ConnectionViewModel()
                        {
                            Input = build(node, IO.Input, FlatShape.Square),
                            Output = build(nodeViewModel, IO.Output, FlatShape.Square),
                            Data = "PropertyConnection"
                        };
                        collection.Add(x);
                    }
                }
                catch (Exception ex)
                {
                }
            });
            return collection;

            IConnectorViewModel build(INodeViewModel nodeViewModel, IO flow, FlatShape shape)
            {
                if (flow == IO.Input)
                {
                    foreach (var x in nodeViewModel.Inputs.Where(a => a.Shape == shape))
                    {
                        return x;
                    }
                }
                else
                {

                    foreach (var x in nodeViewModel.Outputs.Where(a => a.Shape == shape))
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
    }
}