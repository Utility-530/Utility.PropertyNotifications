using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Obs = System.Reactive.Linq.Observable;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Reactive.Disposables;
using Utility.Repos;
using Splat;
using System.Threading.Tasks;
using System.Collections.Generic;
using Utility.Reactives;
using System.Linq;
using Utility.Keys;
using Utility.Structs.Repos;
using Utility.Interfaces.Exs;
using Utility.Changes;
using Utility.Interfaces;
using Utility.Helpers;
using NetFabric.Hyperlinq;
using Utility.Helpers.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Extensions;
using Utility.PropertyNotifications;
using Utility.Nodes.Common;
using Utility.Models;
using Utility.Nodes.Ex;

namespace Utility.Nodes.Filters
{
    public class NodeSource : INodeSource
    {
        public static string New = "new";
        public static readonly string Key = nameof(NodeSource);

        private Dictionary<string, MethodValue> dictionary;
        private Dictionary<string, string> dictionaryMethodNameKeys = [];
        private readonly ObservableCollection<KeyValuePair<string, PropertyChange>> dirtyNodes = [];

        ITreeRepository repository = Locator.Current.GetService<ITreeRepository>();
        Lazy<IFilter> filter = new(() => Locator.Current.GetService<IFilter>());
        Lazy<IExpander> expander = new(() => Locator.Current.GetService<IExpander>());
        Lazy<IContext> context = new(() => Locator.Current.GetService<IContext>());

        private Lazy<Dictionary<string, Setter>> setdictionary = new(() => typeof(Node)
                                                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    .Where(a => a.Name != nameof(IReadOnlyTree.Parent))
                                                    .ToDictionary(a => a.Name, a => Rules.Decide(a)));
        private readonly ObservableCollection<INode> nodes = [];

        private NodeSource()
        {
        }

        public IReadOnlyCollection<INode> Nodes => nodes;
        public ObservableCollection<KeyValuePair<string, PropertyChange>> DirtyNodes => dirtyNodes;
        public static NodeSource Instance { get; } = new();
        string INodeSource.New => New;



        public IObservable<INode> SingleByNameAsync(string name)
        {
            return this.nodes.SelfAndAdditions().Where(a => a.Data.ToString() == name).Take(1);
        }

        public IObservable<INode> ChildrenByGuidAsync(Guid guid)
        {
            return Obs.Create<INode>(observer =>
            {
                try
                {
                    return repository.Find(guid)
                        .Subscribe(a =>
                        {
                            if (a.HasValue == false)
                            {
                                var node = new Node(New);
                                observer.OnNext(node);
                                observer.OnCompleted();
                            }
                            else
                                singleByGuidAsync(a.Value.Guid)
                                .Subscribe(_a =>
                                {
                                    //_a.Name ??= a.Name;
                                    if (_a.Data == "_New_")
                                    {
                                        _a.Data = DataActivator.Activate(a);
                                    }

                                    _a.Removed = a.Value.Removed;
                                    observer.OnNext(_a);
                                });
                        }, () => observer.OnCompleted());
                }
                catch (Exception ex) //when (ex.Message == TreeRepository.No_Existing_Table_No_Name_To_Create_New_One)
                {

                }
                return Disposable.Empty;
            });

            IObservable<INode> singleByGuidAsync(Guid guid)
            {
                return Observable.Create<Node>(observer =>
                {
                    if (Nodes.SingleOrDefault(a => a.Key == new GuidKey(guid)) is not Node node)
                    {
                        node = new Node("_New_") { Key = new GuidKey(guid) };
                    }
                    if (node.Key == null)
                    {
                    }
                    observer.OnNext(node);
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
            }
        }

        public IObservable<INode> FindNodeAsync(Guid parentGuid, Guid currentGuid)
        {
            return Observable.Create<INode>(observer =>
            {
                if (nodes.SingleOrDefault(a => a.Key == new GuidKey(currentGuid)) is Node node)
                {
                    observer.OnNext(node);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                else
                {
                    return repository
                            .Find(parentGuid, guid: currentGuid)
                            .Subscribe(a =>
                            {
                                var node = new Node(DataActivator.Activate(a));
                                observer.OnNext(node);
                                observer.OnCompleted();
                            });
                }
            });
        }

        public DateTime Remove(Guid guid)
        {
            var d = repository.Remove(guid);
            context.Value.UI(() => this.nodes.RemoveBy(a => (GuidKey)a.Key == guid));
            return d;
        }

        public int? MaxIndex(Guid guid, string v)
        {
            return repository.MaxIndex(guid, v);
        }

        public void Remove(INode node)
        {
            this.nodes.Remove(node);
        }

        public IObservable<Structs.Repos.Key?> Find(Guid parentGuid, string name, Guid? guid = null, System.Type? type = null, int? localIndex = null)
        {
            return repository.Find(parentGuid, name, guid, type, localIndex);
        }

        public IObservable<DateValue> Get(Guid guid, string name)
        {
            return repository.Get(guid, name);
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            repository.Set(guid, name, value, dateTime);
        }

        public void Add(INode node)
        {
            if (this.Nodes.Any(a => a.Key == node.Key) == false)
            {
                configure(node);

                repository
                    .Get((GuidKey)node.Key)
                    .Subscribe(_d =>
                    {
                        if (_d.Value != null && setdictionary.Value.TryGetValue(_d.Name, out Setter value))
                            value.Set(node, _d.Value);
                    }, () =>
                    {
                        //Task.Run(() =>
                        //{
                        if (filter.Value?.Filter(node) == false)
                        {
                            node.IsVisible = false;
                        }
                        if (expander.Value?.Expand(node) == true)
                        {
                            node.IsExpanded = true;
                        }
                        //this.nodes.Add(node);
                        node.PropertyChanged += Node_PropertyChanged;
                        node.Items.AndChanges<INode>().Subscribe(a =>
                        {
                            foreach (var x in a)
                                change(x);
                        });
                    });
            }

            async void change(Change a)
            {
                if (a is Change { Type: Changes.Type.Add, Value: { } value })
                {
                    if (value is INode _node)
                        this.nodes.Add(node);
                    else
                    {
                        throw new Exception("11 a 33434");
                    }
                }
                else if (a is Change { Type: Changes.Type.Remove, Value: { } _value })
                {
                    this.nodes.RemoveBy(c =>

                    {
                        if (c is IKey key)
                        {
                            if (_value is IKey _key)
                            {
                                return key.Key.Equals(_key.Key);
                            }
                            else if (_value is IGetGuid guid)
                            {
                                return key.Key.Equals(new GuidKey(guid.Guid));
                            }
                        }
                        throw new Exception("44c dd");

                    });
                }
            }


            void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (sender is INode node)
                {
                    if (e is PropertyChangedExEventArgs { PreviousValue: var previousValue, PropertyName: string name, Value: var value })
                    {
                        context.Value.UI(() => dirtyNodes.Add(new(node.Key, new PropertyChange(sender, name, value, previousValue))));
                    }
                    else
                        throw new Exception("ss FGre333333333");
                }
                else
                    throw new Exception("dfd 4222243");
            }

            void configure(INode node)
            {
                //node.WithChangesTo(a => a.Current)
                //    .Where(a => a != default)
                //    .Subscribe(a =>
                //    {
                //        a.WithChangesTo(a => a.Key)
                //        .Subscribe(key =>
                //        {
                //            try
                //            {
                //                this.Set(Guid.Parse(node.Key), nameof(Node.Current), key, DateTime.Now);
                //            }
                //            catch (Exception ex)
                //            {

                //            }
                //        });
                //    });

                node.WithChangesTo(a => a.Data)
                .Where(a => a is not string)
                .Take(1)
                .Subscribe(data =>
                {
                    if (data is ISetNode iSetNode)
                    {
                        iSetNode.SetNode(node);
                    }
                    if (data is IGetGuid guid && node.Key == null)
                    {
                        node.Key = new GuidKey(guid.Guid);
                    }

                    if (data is IYieldChildren ychildren)
                        _children(ychildren, (GuidKey)node.Key);
                    else
                    {
                        throw new Exception("££ !CC");
                    }

                    //if (data is IChildren children && !(data is IHasChildren { HasChildren: false } hasChildren))
                    //{
                    //    node.WithChangesTo(a => a.Key)
                    //    .Subscribe(key =>
                    //    {
                    //        _children(children, Guid.Parse(key))
                    //        .Filter(node.WithChangesTo(a => a.IsExpanded))
                    //        .Subscribe(change);
                    //    });
                    //}
                });


                IObservable<INode> _children(IYieldChildren children, Guid guid)
                {
                    return Observable.Create<INode>(observer =>
                    {
                        bool b = false;
                        return this
                        .ChildrenByGuidAsync(guid)
                        .Subscribe(a =>
                        {
                            if (a.Data?.ToString() == New || node.Data is ICount)
                            {
                                b = true;
                                //children.Children.Subscribe(a => observer.OnNext(a), () => observer.OnCompleted());
                                children.Children.AndAdditions().Subscribe(async d =>
                                {
                                    var _new = (INode)await node.ToTree(d);

                                    repository
                                    .Find((GuidKey)node.Key, _new.Name())
                                    .Subscribe(a =>
                                    {
                                        if (a.HasValue == false)
                                            throw new Exception("dde33443 21");
                                        _new.Key = new GuidKey(a.Value.Guid);
                                        //node.Add(_new);
                                        observer.OnNext(_new);
                                    });

                                });

                            }
                            else if (a.Data != null && node.Any(n => ((IKey)n).Key == a.Key) == false)
                            {
                                observer.OnNext(a);
                            }
                        },
                        () =>
                        {
                            if (b == false)
                                observer.OnCompleted();
                        });
                    });
                }

                //async void change(Change a)
                //{
                //    if (a is Change { Type: Changes.Type.Add, Value: { } value })
                //    {

                //        if (value is INode _node)
                //        {
                //            node.Add(_node);
                //        }
                //        else
                //        {
                //            _node = (INode)(await node.ToTree(value));

                //        }
                //        node.Add(_node);
                //        this.Add(_node as INode);
                //    }
                //    else if (a is Change { Type: Changes.Type.Remove, Value: { } _value })
                //    {
                //        node.RemoveBy(c =>

                //        {
                //            if (c is IKey key)
                //            {
                //                if (_value is IKey _key)
                //                {
                //                    return key.Key.Equals(_key.Key);
                //                }
                //                else if (_value is IGetGuid guid)
                //                {
                //                    return key.Key.Equals(new GuidKey(guid.Guid));
                //                }
                //            }
                //            throw new Exception("44c dd");

                //        });
                //    }
                //}

                //IObservable<object> _children(IChildren children, Guid guid)
                //{
                //    return Observable.Create<object>(observer =>
                //    {
                //        bool b = false;
                //        return this
                //        .ChildrenByGuidAsync(guid)
                //        .Subscribe(a =>
                //        {
                //            if (a.Data?.ToString() == New || node.Data is ICount)
                //            {
                //                b = true;
                //                children.Children.Subscribe(a => observer.OnNext(a), () => observer.OnCompleted());
                //            }
                //            else if (a.Data != null && node.Any(n => ((IKey)n).Key == a.Key) == false)
                //            {
                //                observer.OnNext(a);
                //            }
                //        },
                //        () =>
                //        {
                //            if (b == false)
                //                observer.OnCompleted();
                //        });
                //    });
                //}
            }

        }

        public async void Save()
        {
            foreach (var item in dirtyNodes.ToArray())
            {
                //var node = item.Value.Source as INode;
                repository.Set(Guid.Parse(item.Key), item.Value.PropertyName, item.Value.Value, DateTime.Now);
                dirtyNodes.Remove(item);
                await Task.Delay(200);
            }
        }

        public IObservable<INode?> Single(string key)
        {
            return Many(key).Take(1);
        }

        public IObservable<INode> Many(string key)
        {
            return MethodCache.Instance.Get(key);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
