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
using Utility.Helpers.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.PropertyDescriptors;
using Utility.Observables;
using Utility.Interfaces.Generic;
using Utility.Trees.Abstractions;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Extensions;

namespace Utility.WPF.Controls.PropertyTrees
{
    public readonly record struct DirtyNode(string Property, INodeViewModel Node);
    public class NodeEngine : Disposable, INodeSource
    {
        public static string New = "new";
        public static readonly string Key = nameof(NodeEngine);

        private Dictionary<string, MethodValue> dictionary;
        private Dictionary<string, string> dictionaryMethodNameKeys = [];
        private readonly ObservableCollection<KeyValuePair<string, PropertyChange>> dirtyNodes = [];

        Lazy<IFilter> filter = new(() => Locator.Current.GetService<IFilter>());
        Lazy<IExpander> expander = new(() => Locator.Current.GetService<IExpander>());
        Lazy<IContext> context = new(() => Locator.Current.GetService<IContext>());

        private readonly ObservableCollection<INodeViewModel> nodes = [];

        private NodeEngine()
        {
        }

        public IReadOnlyCollection<INodeViewModel> Nodes => nodes;
        public ObservableCollection<KeyValuePair<string, PropertyChange>> DirtyNodes => dirtyNodes;
        public static NodeEngine Instance { get; } = new();
        string INodeSource.New => New;

        public IObservable<INodeViewModel> Selections { get; }

        public IObservable<INodeViewModel?> Single(string key)
        {
            throw new NotImplementedException();
            //return Many(key).Take(1);
        }

        public IObservable<INodeViewModel> SingleByNameAsync(string name)
        {
            return nodes.SelfAndAdditions().Where(a => a.Data().ToString() == name).Take(1);
        }

        public IObservable<INodeViewModel> ChildrenByGuidAsync(Guid guid)
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

        public void Remove(INodeViewModel node)
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
            throw new NotImplementedException();
            //return repository.Get(guid, name);
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            // repository.Set(guid, name, value, dateTime);
        }

        public void Add(INodeViewModel node)
        {
            if (node.Data() is not IDescriptor descriptor)
                throw new Exception("Vd 222");
            if ((node as IGetKey).Key is null)
                (node as ISetKey).Key = new GuidKey(Guid.NewGuid());
            if (Nodes.Any(a => (a as IGetKey).Key == (node as IGetKey).Key) == false)
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
                } //);

                configure(node);

            }

            void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (sender is INodeViewModel node)
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

            void configure(INodeViewModel node)
            {
                if ((node as IGetParent<IReadOnlyTree>).Parent?.Data() is ICollectionDescriptor d)
                {

                }
                node.WithChangesTo(a => (a as IGetData).Data)
                .Where(a => a is not string)
                .Take(1)
                .Subscribe(data =>
                {
                    if ((node as IGetParent<IReadOnlyTree>).Parent?.Data() is ICollectionDescriptor d)
                    {

                    }
                    //if (data is ISetNode iSetNode)
                    //{
                    //    iSetNode.SetNode(node);
                    //}
                    if (data is IGetGuid guid && (node as IGetKey).Key == null)
                    {
                        (node as ISetKey).Key = new GuidKey(guid.Guid);
                    }


                    if (data is IYieldItems ychildren)
                        ychildren.Items().Cast<IDescriptor>().ForEach(async d =>
                        {
                            var newNode = await node.ToTree(d);
                            node.Add(newNode);
                            if (newNode.Data() is IGetName { Name: "BaseType" })
                            {

                            }
                            Add(newNode as INodeViewModel);
                        });
                    else
                    {

                    }

                });

                async void change(Change a)
                {
                    if (a is Change { Type: Changes.Type.Add, Value: { } value })
                    {
                        if (value is INodeViewModel _node)
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
                                    return (key as IGetKey).Key.Equals((_key as IGetKey).Key);
                                }
                                else if (_value is IGetGuid guid)
                                {
                                    return (key as IGetKey).Key.Equals(new GuidKey(guid.Guid));
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

        public IObservable<INodeViewModel> Many(string key)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }



        public IObservable<INodeViewModel> FindChild(INodeViewModel node, Guid guid)
        {
            throw new NotImplementedException();
        }

        public IObservable<INodeViewModel> Create(string name, Guid guid, Func<string, object> modelFactory)
        {
            throw new NotImplementedException();
        }

        public void RemoveBy(Predicate<INodeViewModel> predicate)
        {
            throw new NotImplementedException();
        }
    }

    public class MethodValue
    {
        public MethodInfo Method { get; set; }

        public Task Task { get; set; }

        public IList<INodeViewModel> Nodes { get; set; } = [];
    }
}
