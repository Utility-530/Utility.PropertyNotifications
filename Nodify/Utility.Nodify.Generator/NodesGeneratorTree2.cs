using DryIoc;
using System.Collections.ObjectModel;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Core;
using Utility.Nodify.Enums;
using Utility.Nodify.Models;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Trees.Abstractions;

namespace Nodify.Playground
{
    public class NodesGeneratorTree2<T> where T : NodeViewModel, new()
    {
        ObservableCollection<T> nodes = [];
        ObservableCollection<ConnectionViewModel> connections = [];

        public ObservableCollection<T> GenerateNodes(NodesGeneratorSettings settings)
        {
            uint i = 0;
            (Utility.Globals.Resolver.Resolve<IObservable<IReadOnlyTree>>())
                .Subscribe(item =>
                {
                    var index = Utility.Trees.Extensions.Generic.Index(item, item => (item as IGetParent<IReadOnlyTree>).Parent, (item, child) => Utility.Helpers.NonGeneric.Linq.IndexOf((item as IReadOnlyTree).Items, a => (a as IGetKey).Key.Equals((child as IGetKey).Key)));
                    var newIndex = new Utility.Structs.Index(index.ToArray());
                    if (item is ITreeIndex { Index:{ } _index } treeIndex)
                    {
                        if(newIndex.Equals(_index) ==false)
                        {

                        }
                    }

                    var node = new T
                    {
                        Data = item,
                        //Location = settings.NodeLocationGenerator(settings, ++i),
                        Key = newIndex.ToString()
                    };

                    nodes.Add(node);

                });

            return nodes;
        }

        T? root = null;
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
                        Utility.Trees.Extensions.Generic.ToTree<ObservableCollection<T>, T, string, ConnectionViewModel>(
                        nodes,
                        n => idSelector(n),
                        n => parentIdSelector(n),
                        (a, b) =>
                        {
                            try
                            {
                                return new ConnectionViewModel()
                                {
                                    Input = build(b?.Output?.Node ?? root, ConnectorFlow.Output, ConnectorShape.Square),
                                    Output = build(a, ConnectorFlow.Input, ConnectorShape.Square),
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
            return idSelector(n.Data);
            throw new Exception("d3fff");
        }

        private static string idSelector(object n)
        {
            if (n is IGetName _name)
                return _name.Name;

            if (n is IData { Data: IGetName{ Name: { } name  } })
                return name;
            throw new Exception("d3fff");
        }

        private static string? parentIdSelector(T n)
        {
            if (n.Data is IGetParent<IReadOnlyTree> { Parent: { }  parent })
            {
                return idSelector(parent);
            }
            return null;
            // root
        }

        private T? _root(IEnumerable<object> enumerable)
        {
            foreach (var node in enumerable)
            {
                if (node is IModel model)
                {
                    if ((model as IGetParent<IModel>).Parent == null)
                    {
                        var _node = new T { Data = model, Key = "0" };
                        var output = new ConnectorViewModel { Node = _node, Shape = ConnectorShape.Square, Flow = ConnectorFlow.Output, Key = "output", Data = null };
                        Shared.serviceConnectors.Add(_node, output);
                        _node.Output.Add(output);
                        output.Node = _node;
                        return _node;
                    }
                    else if (this._root([(model as IGetParent<IModel>).Parent]) is T t)
                        return t;
                }
            }
            return null;
        }
    }
}