using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reflection;
using Splat;
using Utility.Reactives;
using Utility.Keys;
using Utility.Structs.Repos;
using Utility.Interfaces.Exs;
using Utility.Changes;
using Utility.Interfaces;
using Utility.Helpers;
using System.Linq;
using Utility.Helpers.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Helpers.NonGeneric;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Utility.Nodes.Ex;
using Utility.Extensions;
using Utility.Models.Trees;
using Utility.Trees.Abstractions;
using Utility.Models;
using Fasterflect;
using System.ComponentModel;


namespace Utility.Nodes.Filters
{
    public readonly record struct DirtyNode(string Property, INode Node);
    public class NodeEngine : INodeSource
    {
        public static string New = "new";
        public static readonly string Key = nameof(NodeEngine);

        Lazy<IFilter> filter = new(() => Locator.Current.GetService<IFilter>());
        Lazy<IExpander> expander = new(() => Locator.Current.GetService<IExpander>());
        Lazy<IContext> context = new(() => Locator.Current.GetService<IContext>());
        Lazy<ITreeRepository> repository = new(() => Locator.Current.GetService<ITreeRepository>());

        private Lazy<Dictionary<string, Setter>> setdictionary = new(() => typeof(Node)
                                                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    .Where(a => a.Name != nameof(IReadOnlyTree.Parent))
                                                    .ToDictionary(a => a.Name, a => Rules.Decide(a)));

        private readonly ObservableCollection<INode> nodes = [];

        private NodeEngine()
        {
        }

        public IReadOnlyCollection<INode> Nodes => nodes;
        //public ObservableCollection<KeyValuePair<string, PropertyChange>> DirtyNodes => dirtyNodes;
        public static NodeEngine Instance { get; } = new();
        string INodeSource.New => New;



        public IObservable<INode> SingleByNameAsync(string name)
        {
            return nodes.SelfAndAdditions().Where(a => a.Data.ToString() == name).Take(1);
        }


        public DateTime Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public int? MaxIndex(Guid guid, string v)
        {
            //return repository.MaxIndex(guid, v);
            throw new NotImplementedException();

        }

        public void Remove(INode node)
        {
            nodes.Remove(node);
        }

        public IObservable<Structs.Repos.Key?> Find(Guid parentGuid, string name, Guid? guid = null, System.Type type = null, int? localIndex = null)
        {
            throw new NotImplementedException();

            // return repository.Find(parentGuid, name, guid, type, localIndex);
        }

        public IObservable<DateValue> Get(Guid guid, string name)
        {
            return repository.Value.Get(guid, name);
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            repository.Value.Set(guid, name, value, dateTime);
        }

        public void Add(INode node)
        {
            if (node.Key is null)
            {
                var index = node.Parent.Items.Count(a => ((INode)a).Name() == node.Name());
                var type = node.Data is IGetType { } getType ? getType.GetType() : node.Data.GetType();
                repository
                    .Value
                    .Find((GuidKey)node.Parent.Key, node.Name(), type: type, index: index == 0 ? null : index)
                    .Subscribe(_key =>
                    {
                        if (_key.HasValue == false)
                            throw new Exception("dde33443 21");
                        node.Key = new GuidKey(_key.Value.Guid);
                        Add(node);
                    });
                return;
            }
            else
            {
                if (Nodes.Any(a => a.Key == node.Key) == false)
                {
                    nodes.Add(node);
                    configure(node);
                }
                else
                {

                }
            }


            void configure(INode node)
            {
                if (filter.Value?.Filter(node) == false)
                {
                    node.IsVisible = false;
                    return;
                }
                if (expander.Value?.Expand(node) == true)
                {
                    node.IsExpanded = true;
                }

                node.Items.AndChanges<INode>().Subscribe(a =>
                {
                    foreach (var x in a)
                        change(x);
                });

                void node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    if (sender is INode node)
                    {
                        if (e is PropertyChangedExEventArgs { PreviousValue: var previousValue, PropertyName: string name, Value: var value })
                        {
                            context.Value.UI(() =>
                            {
                                Single(nameof(Factory.BuildDirty))
                                .Subscribe(async _node =>
                                {
                                    if (_node.Data is not CollectionModel<DirtyModel> { Collection: { } collection } exceptionsModel)
                                        throw new Exception("775 333");
                                    else
                                        _node.Add(await _node.ToTree(new DirtyModel { Name = name + node.Items.Count(), SourceKey = node.Key, PropertyName = name, NewValue = value }));
                                });
                            });
                        }
                        else
                            throw new Exception("ss FGre333333333");
                    }
                    else
                        throw new Exception("dfd 4222243");
                }


                void change(Change a)
                {
                    if (a is Change { Type: Changes.Type.Add, Value: { } value })
                    {
                        if (value is INode _node)
                            Add(_node);
                        else
                        {
                            //node.Add(await node.ToTree(value));
                        }
                    }
                    else if (a is Change { Type: Changes.Type.Remove, Value: { } _value })
                    {
                        nodes.RemoveBy<INode>(c =>

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


                node
                    .WithChangesTo(a => a.Data)
                    .Where(a => a is not string)
                    .Take(1)
                    .Subscribe(data =>
                    {
                        load(node).Subscribe(a =>
                        {
                            node.PropertyChanged += node_PropertyChanged;
                            if (data is INotifyPropertyCalled called)
                            {
                                called.WhenCalled()
                                .Subscribe(call =>
                                {
                                    repository.Value
                                    .Get((GuidKey)node.Key, nameof(Node.Data) + "." + call.Name)
                                    .Subscribe(a =>
                                    {
                                        if (a.Value != null)
                                        {
                                            if (call.Target.TryGetFieldValue(call.Name.ToLower()).Equals(a.Value) == true)
                                                return;
                                            call.Target.TrySetValue(call.Name, a.Value);
                                            if (call.Target is INotifyPropertyChanged changed)
                                                changed.RaisePropertyChanged(call.Name);
                                        }
                                    });
                                });
                            }
                            if (data is INotifyPropertyReceived received)
                            {
                                received.WhenReceived()
                                .Where(a => a.Value != null)
                                .Subscribe(reception =>
                                {
                                    repository.Value.Set((GuidKey)node.Key, nameof(Node.Data) + "." + reception.Name, reception.Value, DateTime.Now);
                                });
                            }


                            if (data is ISetNode iSetNode)
                            {
                                iSetNode.SetNode(node);
                            }
                            else
                            {
                                //throw new Exception("R333 ");
                            }



                            node.WithChangesTo(a => a.IsExpanded)
                            .Where(a => a == true)
                            .Take(1)
                                .Subscribe(a =>
                                {
                                    if (data is IYieldChildren ychildren)
                                        _children(node, ychildren, (GuidKey)node.Key).Subscribe(a =>
                                        {
                                            node.Add(a);
                                        });


                                });
                        });


                    });


                IObservable<INode> _children(INode node, IYieldChildren children, Guid guid)
                {
                    return Observable.Create<INode>(observer =>
                    {
                        return repository.Value.Find(guid)
                            .Subscribe(async key =>
                            {

                                if (key.HasValue == false)
                                {
                                    children
                                    .Children
                                    .AndAdditions()
                                    .Subscribe(async d =>
                                    {
                                        var _new = (INode)await node.ToTree(d);
                                        if(d is IIsReadOnly readOnly)
                                        {
                                            (_new as ISetIsReadOnly).IsReadOnly = readOnly.IsReadOnly;  
                                        }
                                        repository
                                        .Value
                                        .Find((GuidKey)node.Key, _new.Name(), type: d.GetType())
                                        .Subscribe(_key =>
                                        {
                                            if (_key.HasValue == false)
                                                throw new Exception("dde33443 21");
                                            _new.Key = new GuidKey(_key.Value.Guid);
                                            observer.OnNext(_new);
                                        });
                                    });
                                }
                                else
                                {
                                    if (Nodes.SingleOrDefault(a => a.Key == new GuidKey(key.Value.Guid)) is not Node _node)
                                    {
                                        var _new = (INode)await node.ToTree(DataActivator.Activate(key));
                                        _new.Key = new GuidKey(key.Value.Guid);
                                        _new.Removed = key.Value.Removed;
                                        observer.OnNext(_new);
                                    }
                                    else
                                    {
                                        throw new Exception("u 333333312");
                                    }

                                }
                            }, () => observer.OnCompleted());

                    });
                }
            }
        }


        IObservable<INode> load(INode node)
        {
            return Observable.Create<INode>(observer =>
            {
                return repository.Value.Get(Guid.Parse(node.Key)).Subscribe(_d =>
                {
                    if (_d.Name == null)
                    {
                    }
                    else if (_d.Value != null && setdictionary.Value.TryGetValue(_d.Name, out Setter value))
                        value.Set(node, _d.Value);
                }, () =>
                {
                    observer.OnNext(node);
                    observer.OnCompleted();
                });
            });
        }

        public void Save()
        {
            Single(nameof(Factory.BuildDirty))
                .Subscribe(async tree =>
                {
                    if (tree.Data is not CollectionModel<DirtyModel> model)
                        throw new Exception("DSF 64333");
                    foreach (var item in tree.ToArray())
                    {
                        if (item.Data is DirtyModel { SourceKey: { } sk, PropertyName: { } pn, NewValue: { } nv } dmodel)
                        {
                            //var node = item.Value.Source as INode;
                            repository.Value.Set(Guid.Parse(sk), pn, nv, DateTime.Now);
                            tree.Remove(item);
                            await Task.Delay(200);
                        }
                    }
                });
        }

        public IObservable<INode> Single(string key)
        {
            return Many(key).Take(1);
        }

        public IObservable<INode> Many(string key)
        {
            if (Nodes.SingleOrDefault(a => a.Key.Equals(key)) is Node node)
            {
                return Observable.Return(node);
            }

            return MethodCache.Instance.Get(key);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class MethodValue
    {
        public MethodInfo Method { get; set; }

        public Task Task { get; set; }

        public IList<INode> Nodes { get; set; } = [];
    }
}
