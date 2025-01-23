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
using Utility.Repos;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Interfaces.Exs;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class ControlsService
    {
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
                        foreach (INode INode in _n)
                        {
                            if (INode is { Data: Model { Name: string name } model })
                            {
                                model.WhenAnyPropertyChanged().Subscribe(_ =>
                                {
                                    var contentRoot = root.MatchDescendant(a => (a as IName).Name == Factory.content_root);
                                    Switch(name, model, contentRoot as INode);
                                });
                            }
                        }
                    });
                });
        }

        private void Switch(string name, Model model, INode? contentRoot)
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

        //public void Search(INode node, string value)
        //{
        //    foreach (INode n in node.Descendants().Reverse())
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

        public void Save(INode node)
        {
            NodeSource.Instance.Save();
        //    NodeSource.Instance.
        //        Single(nameof(Factory.BuildFiltersRoot))
        //        .Subscribe(a =>
        //        {
        //            a.Foreach((_n, i) =>
        //            {
        //                try
        //                {
        //                    if ((_n as IIsPersistable).IsPersistable == false)
        //                    {
        //                    }
        //                    else
        //                    {
        //                        //var INode = Locator.Current.GetService<IMapper>().Map<INode, NodeDTO>(_n as INode);
        //                        //TreeRepository.Instance.Set(INode.Guid, INode, DateTime.Now);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
        //                    .Subscribe(a =>
        //                    {
        //                        a.Items.AndAdditions<ITree>().Subscribe(node =>
        //                        {
        //                            if (node is { Data: ExceptionsModel { Name: string name } model })
        //                            {
        //                                node.Add(new Node(ExceptionModel.Create(ex)));
        //                            }
        //                        });
        //                    });
        //                }
        //            });
        //        });
        //    NodeSource.Instance.
        //Single(nameof(Factory.BuildComboRoot))
        //.Subscribe(a =>
        //{
        //    a.Foreach((_n, i) =>
        //    {
        //        try
        //        {
        //            if ((_n as IIsPersistable).IsPersistable == false)
        //            {
        //            }
        //            else
        //            {
        //                //var INode = Locator.Current.GetService<IMapper>().Map<INode, NodeDTO>(_n as INode);
        //                //TreeRepository.Instance.Set(INode.Guid, INode, DateTime.Now);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            NodeSource.Instance.Single(nameof(Factory.BuildControlRoot))
        //            .Subscribe(a =>
        //            {
        //                a.Items.AndAdditions<ITree>().Subscribe(node =>
        //                {
        //                    if (node is { Data: ExceptionsModel { Name: string name } model })
        //                    {
        //                        node.Add(new Node(ExceptionModel.Create(ex)));
        //                    }
        //                });
        //            });
        //        }
        //    });
        //});
        }

        public void SaveFilters(INode node)
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
                                //var INode = Locator.Current.GetService<IMapper>().Map<INode, NodeDTO>(_n as INode);
                                //TreeRepository.Instance.Set(INode.Guid, INode, DateTime.Now);
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
                                        node.Add(new Node(ExceptionModel.Create(ex)));
                                    }
                                });
                            });
                        }
                    });
                });

        }

        //    public async void Load(INode INode)
        //    {
        //        if (INode == null)
        //        {
        //            NodeSource.Instance.Single(nameof(Factory.BuildContentRoot))
        //                .Subscribe(a =>
        //                {
        //                    selectKeys(a);
        //                });
        //        }
        //        else
        //        {
        //            selectKeys(INode);
        //        }
        //        void selectKeys(INode node)
        //        {
        //            TreeRepository.Instance.SelectKeys(table_name: Factory.content_root)
        //                .Subscribe(keys =>
        //                {
        //                    foreach (var key in keys)
        //                    {
        //                        if (node.MatchDescendant(a => (a as IGuid).Guid.Equals(key.Guid)) is INode)
        //                        {
        //                            continue;
        //                        }
        //                        if (node.MatchDescendant(a => (a as IGuid).Guid.Equals(key.ParentGuid)) is INode parentNode)
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

        //        void load(Guid guid, INode parentNode)
        //        {
        //            TreeRepository.Instance.Get(guid).Subscribe(get =>
        //            {
        //                if (get.Value == null)
        //                    return;
        //                var INode = Locator.Current.GetService<IMapper>().Map<NodeDTO, INode>((NodeDTO)get.Value);

        //                TreeRepository.Instance.Find(guid)
        //                .Subscribe(parentGuid =>
        //                {
        //                    NodeSource.Instance.FindNodeAsync(parentGuid.Guid)
        //                    .Subscribe(parent =>
        //                    {
        //                        //var parent = parentNode.Find(a => a.Guid.Equals(parentGuid));
        //                        INode.Parent = parent;
        //                        INode.IsEditable = true;
        //                        parent.Add(INode);

        //                        if (disposables.TryGetValue(guid, out IDisposable? disposable))
        //                            disposable?.Dispose();

        //                        disposables[guid] = INode.WithChanges()
        //                                                .Subscribe(ac =>
        //                                                {
        //                                                    //Queue(ac);
        //                                                    dirtyNodes[INode.Guid] = true;
        //                                                });
        //                        dirtyNodes[INode.Guid] = false;

        //                    });


        //                });


        //            });
        //        }
        //    }

        //    public void Clear(INode node)
        //    {
        //        node.Clear();
        //    }

        //    public void New(INode node)
        //    {

        //        NodeSource.Instance.Single(nameof(Factory.BuildDefault))
        //            .Subscribe(r =>
        //            r.Foreach((a, i) =>
        //            {
        //                (a as IIsEditable).IsEditable = true;
        //            }));
        //    }

        //    public void Expand(INode node)
        //    {
        //        node.Foreach((a, i) => (a as IIsExpanded).IsExpanded = true);
        //    }

        //    public void Collapse(INode node)
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
