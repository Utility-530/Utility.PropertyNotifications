using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using AutoMapper;
using DynamicData;
using DynamicData.Binding;
using Splat;
using Utility.Extensions;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Filters;
using _Node = Utility.Nodes.Filters.Node;
using Utility.Repos;
using Utility.Reactives;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class ControlsService
    {
        Dictionary<Guid, bool> dirtyNodes = new();
        Dictionary<Guid, IDisposable> disposables = new();

        public ControlsService()
        {
            NodeSource.Instance
                .Many(nameof(Factory.BuildRoot))
                .Subscribe(root =>
                {
                    NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
                    .Subscribe(_n =>
                    {
                        foreach (_Node _node in _n)
                        {
                            if (_node is { Data: Model { Name: string name } model })
                            {
                                model.WhenAnyPropertyChanged().Subscribe(_ =>
                                {
                                    var contentRoot = root.MatchDescendant(a => (a as IName).Name == Factory.content_root);
                                    Switch(name, model, contentRoot as _Node);
                                });
                            }
                        }
                    });
                });
        }

        private void Switch(string name, Model model, _Node? contentRoot)
        {
            switch (name)
            {
                case Factory.Save:
                    Save(contentRoot);
                    break;
                case Factory.Save_Filters:
                    SaveFilters(contentRoot);
                    break;
                //case Factory.Load:
                //    Load(contentRoot);
                //    break;
                //case Factory.New:
                //    New(contentRoot);
                //    break;
                //case Factory.Clear:
                //    Clear(contentRoot);
                //    break;
                //case Factory.Expand:
                //    Expand(contentRoot);
                //    break;
                //case Factory.Collapse:
                //    Collapse(contentRoot);
                //    break;
                case Factory.Refresh:
                    //Refresh(contentRoot);
                    break;
                    //case Factory.Search when model is SearchModel { SearchText: { } searchText } searchModel:
                    //    Search(contentRoot, searchText);

                    break;
            }
        }

        //public void Search(_Node node, string value)
        //{
        //    foreach (_Node n in node.Descendants().Reverse())
        //    {
        //        var x =
        //         n.Data is Model model && model.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase);
        //        if (!x)
        //        {
        //            if (n.Items.Any() == false)
        //            {
        //                n.IsVisible = false;
        //            }
        //            else if (n.All(a => (a as IIsVisible).IsVisible == false))
        //            {
        //                n.IsVisible = false;
        //            }
        //            else
        //            {
        //                n.IsHighlighted = false;
        //            }
        //        }
        //        else
        //        {
        //            n.IsHighlighted = true;
        //            n.IsVisible = true;
        //        }
        //    }
        //}

        public void Save(_Node node)
        {
            NodeSource.Instance.
                Single(nameof(Factory.BuildFiltersRoot))
                .Subscribe(a =>
                {
                    a.Foreach((_n, i) =>
                    {

                        try
                        {
                            if ((_n as IIsPersistable).IsPersistable == false)
                            {

                            }
                            else
                            {
                                var _node = Locator.Current.GetService<IMapper>().Map<_Node, NodeDTO>(_n as _Node);
                                TreeRepository.Instance.Set(_node.Guid, _node, DateTime.Now);
                            }
                        }
                        catch (Exception ex)
                        {
                            NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
                            .Subscribe(a =>
                            {
                                a.Items.AndAdditions<ITree>().Subscribe(node =>
                                {
                                    if (node is { Data: ExceptionsModel { Name: string name } model })
                                    {
                                        node.Add(new _Node(ex.Message, new ExceptionModel(ex)));
                                    }
                                });
                            });
                        }

                        dirtyNodes[(_n as IGuid).Guid] = false;
                    });
                });
            NodeSource.Instance.
        Single(nameof(Factory.BuildComboRoot))
        .Subscribe(a =>
        {
            a.Foreach((_n, i) =>
            {

                try
                {
                    if ((_n as IIsPersistable).IsPersistable == false)
                    {

                    }
                    else
                    {
                        var _node = Locator.Current.GetService<IMapper>().Map<_Node, NodeDTO>(_n as _Node);
                        TreeRepository.Instance.Set(_node.Guid, _node, DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
                    .Subscribe(a =>
                    {
                        a.Items.AndAdditions<ITree>().Subscribe(node =>
                        {
                            if (node is { Data: ExceptionsModel { Name: string name } model })
                            {
                                node.Add(new _Node(ex.Message, new ExceptionModel(ex)));
                            }
                        });
                    });
                }

                dirtyNodes[(_n as IGuid).Guid] = false;
            });
        });

        }

        public void SaveFilters(_Node node)
        {
            {
                NodeSource.Instance.
                    Single(nameof(Factory.BuildContentRoot))
                    .Subscribe(a =>
                    {
                        a.Foreach((_n, i) =>
                        {


                            try
                            {
                                if ((_n as IIsPersistable).IsPersistable == false)
                                {

                                }
                                else
                                {
                                    var _node = Locator.Current.GetService<IMapper>().Map<_Node, NodeDTO>(_n as _Node);
                                    TreeRepository.Instance.Set(_node.Guid, _node, DateTime.Now);
                                }
                            }
                            catch (Exception ex)
                            {
                                NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
                                .Subscribe(a =>
                                {
                                    a.Items.AndAdditions<ITree>().Subscribe(node =>
                                    {
                                        if (node is { Data: ExceptionsModel { Name: string name } model })
                                        {
                                            node.Add(new _Node(ex.Message, new ExceptionModel(ex)));
                                        }
                                    });
                                });
                            }

                            dirtyNodes[(_n as IGuid).Guid] = false;
                        });
                    });
            }
        }

        //    public async void Load(_Node _node)
        //    {
        //        if (_node == null)
        //        {
        //            NodeSource.Instance.Single(nameof(Factory.BuildContentRoot))
        //                .Subscribe(a =>
        //                {
        //                    selectKeys(a);
        //                });
        //        }
        //        else
        //        {
        //            selectKeys(_node);
        //        }
        //        void selectKeys(_Node node)
        //        {
        //            TreeRepository.Instance.SelectKeys(table_name: Factory.content_root)
        //                .Subscribe(keys =>
        //                {
        //                    foreach (var key in keys)
        //                    {
        //                        if (node.MatchDescendant(a => (a as IGuid).Guid.Equals(key.Guid)) is _Node)
        //                        {
        //                            continue;
        //                        }
        //                        if (node.MatchDescendant(a => (a as IGuid).Guid.Equals(key.ParentGuid)) is _Node parentNode)
        //                        {
        //                            load(key.Guid, parentNode);
        //                        }
        //                        else
        //                        {
        //                            load(key.Guid, node);
        //                        }
        //                    }
        //                });
        //        }

        //        void load(Guid guid, _Node parentNode)
        //        {
        //            TreeRepository.Instance.Get(guid).Subscribe(get =>
        //            {
        //                if (get.Value == null)
        //                    return;
        //                var _node = Locator.Current.GetService<IMapper>().Map<NodeDTO, _Node>((NodeDTO)get.Value);

        //                TreeRepository.Instance.Find(guid)
        //                .Subscribe(parentGuid =>
        //                {
        //                    NodeSource.Instance.FindNodeAsync(parentGuid.Guid)
        //                    .Subscribe(parent =>
        //                    {
        //                        //var parent = parentNode.Find(a => a.Guid.Equals(parentGuid));
        //                        _node.Parent = parent;
        //                        _node.IsEditable = true;
        //                        parent.Add(_node);

        //                        if (disposables.TryGetValue(guid, out IDisposable? disposable))
        //                            disposable?.Dispose();

        //                        disposables[guid] = _node.WithChanges()
        //                                                .Subscribe(ac =>
        //                                                {
        //                                                    //Queue(ac);
        //                                                    dirtyNodes[_node.Guid] = true;
        //                                                });
        //                        dirtyNodes[_node.Guid] = false;

        //                    });


        //                });


        //            });
        //        }
        //    }

        //    public void Clear(_Node node)
        //    {
        //        node.Clear();
        //    }

        //    public void New(_Node node)
        //    {

        //        NodeSource.Instance.Single(nameof(Factory.BuildDefault))
        //            .Subscribe(r =>
        //            r.Foreach((a, i) =>
        //            {
        //                (a as IIsEditable).IsEditable = true;
        //            }));
        //    }

        //    public void Expand(_Node node)
        //    {
        //        node.Foreach((a, i) => (a as IIsExpanded).IsExpanded = true);
        //    }

        //    public void Collapse(_Node node)
        //    {
        //        node.Foreach((a, i) => (a as IIsExpanded).IsExpanded = false);
        //    }
        //    IDisposable? disposable;

        //    int i = 0;

    }

    public static class Helpers
    {
        public static IEnumerable<T> Select<T>(this ArrayList list, Func<object, T> map)
        {
            foreach (var x in list)
            {
                yield return map(x);
            }
        }

        public static HashSet<T> ToHashSet<T>(Collection<T> collection)
        {
            return new HashSet<T>(collection);
        }
    }
}
