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
using Utility.Nodes.Filters;
using Utility.Helpers.NonGeneric;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Utility.Nodes.Demo.Filters
{
    public readonly record struct DirtyNode(string Property, INode Node);
    public class NodeEngine : INodeSource
    {
        public static string New = "new";
        public static readonly string Key = nameof(NodeEngine);

        private Dictionary<string, MethodValue> dictionary;
        private Dictionary<string, string> dictionaryMethodNameKeys = [];
        private readonly ObservableCollection<KeyValuePair<string, PropertyChange>> dirtyNodes = [];

        Lazy<IFilter> filter = new(() => Locator.Current.GetService<IFilter>());
        Lazy<IExpander> expander = new(() => Locator.Current.GetService<IExpander>());
        Lazy<IContext> context = new(() => Locator.Current.GetService<IContext>());
        Lazy<ITreeRepository> repository = new(() => Locator.Current.GetService<ITreeRepository>());

        private readonly ObservableCollection<INode> nodes = [];

        private NodeEngine()
        {
        }

        public IReadOnlyCollection<INode> Nodes => nodes;
        public ObservableCollection<KeyValuePair<string, PropertyChange>> DirtyNodes => dirtyNodes;
        public static NodeEngine Instance { get; } = new();
        string INodeSource.New => New;



        public IObservable<INode> SingleByNameAsync(string name)
        {
            return nodes.SelfAndAdditions().Where(a => a.Data.ToString() == name).Take(1);
        }

        public IObservable<INode> ChildrenByGuidAsync(Guid guid)
        {
            throw new NotImplementedException();
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

        public IObservable<Structs.Repos.Key?> Find(Guid parentGuid, string name, Guid? guid = null, System.Type? type = null, int? localIndex = null)
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
            //if (node.Data is not IDescriptor descriptor)
            //    throw new Exception("Vd 222");
            if (node.Key is null)
                node.Key = new GuidKey(Guid.NewGuid());
            if (Nodes.Any(a => a.Key == node.Key) == false)
            {
                nodes.Add(node);
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

                    node.PropertyChanged += Node_PropertyChanged;
                    node.Items.AndAdditions<INode>().Subscribe(a =>
                    {
                        this.Add(a);
                    });
                } //);

                configure(node);

            }

            void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (sender is INode node)
                {
                    if (e is PropertyChangedExEventArgs { PreviousValue: var previousValue, PropertyName: string name, Value: var value })
                    {
                        //context.Value.UI(() => dirtyNodes.Add(new(node.Key, new PropertyChange(sender, name, value, previousValue))));
                    }
                    else
                        throw new Exception("ss FGre333333333");
                }
                else
                    throw new Exception("dfd 4222243");
            }

            void Node_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (sender is INode node)
                {
                    if (e.NewItems?.Count > 0)
                    {
                        foreach (var newItem in e.NewItems)
                        {
                            Add(newItem as INode);
                        }
                        //context.Value.UI(() => dirtyNodes.Add(new(node.Key, new PropertyChange(sender, name, value, previousValue))));
                    }
                    else
                        throw new Exception("ss FGre333333333");
                }
                else
                    throw new Exception("dfd 4222243");
            }



            void configure(INode node)
            {

                node
                    .WithChangesTo(a => a.Data)
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
                        else
                        {

                        }

                        node.WithChangesTo(a => a.IsExpanded)
                        .Where(a => a == true)
                        .Take(1)
                            .Subscribe(a =>
                            {
                                if (data is IYieldChildren ychildren)
                                    ychildren.Children.ForEach(async d =>
                                    {
                                        if (node.Any(a => (a.Data as IGetName).Name == (d as IGetName).Name))
                                        {
                                            return;
                                        }
                                        var newNode = await node.ToTree(d);
                                        node.Add(newNode);
                                        if (newNode.Data is IGetName { Name: "BaseType" })
                                        {

                                        }
                                        Add(newNode as INode);
                                    });
                            });
                    });


                IObservable<object> _children(IChildren children, Guid guid)
                {
                    return Observable.Create<object>(observer =>
                    {
                        bool b = false;
                        return this
                        .ChildrenByGuidAsync(guid)
                        .Subscribe(a =>
                        {
                            if (a.Data?.ToString() == New || node.Data is ICount)
                            {
                                b = true;
                                children.Children.Subscribe(a => observer.OnNext(a), () => observer.OnCompleted());
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

                async void change(Change a)
                {
                    if (a is Change { Type: Changes.Type.Add, Value: { } value })
                    {
                        if (value is INode _node)
                            node.Add(_node);
                        else
                        {
                            node.Add(await node.ToTree(value));
                        }
                    }
                    else if (a is Change { Type: Changes.Type.Remove, Value: { } _value })
                    {
                        node.RemoveBy(c =>

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
            }
        }


        public async void Save()
        {
            throw new NotImplementedException();
        }

        public IObservable<INode?> Single(string key)
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
