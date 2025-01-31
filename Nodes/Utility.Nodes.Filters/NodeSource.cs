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

namespace Utility.Nodes.Filters
{
    public readonly record struct DirtyNode(string Property, INode Node);
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
        private ObservableCollection<INode> nodes = [];

        private NodeSource()
        {
        }

        public IReadOnlyCollection<INode> Nodes => nodes;
        public ObservableCollection<KeyValuePair<string, PropertyChange>> DirtyNodes => dirtyNodes;
        public static NodeSource Instance { get; } = new();
        string INodeSource.New => New;

        public IObservable<INode?> Single(string key)
        {
            return Many(key).Take(1);
        }

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
                    return repository.Find(parentGuid, guid: currentGuid).Subscribe(a =>
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
            context.Value.UI(() =>
       this.nodes.RemoveBy(a => (GuidKey)a.Key == guid));
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
                this.nodes.Add(node);

                repository.Get((GuidKey)node.Key)
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

                        node.PropertyChanged += Node_PropertyChanged;
                    });
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

        public IObservable<INode> Many(string key)
        {
            dictionary ??= typeof(Factory)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .ToDictionary(a => a.Name, a => new MethodValue { Method = a });

            if (dictionaryMethodNameKeys.ContainsKey(key) == false)
            {
                var output = dictionary[key].Method.Invoke(null, Array.Empty<object>());
                if (output is Node node)
                {
                    dictionary[key].Nodes.Add(node);
                    dictionaryMethodNameKeys[key] = node.Data.ToString();
                    //node.Name = key;
                }
                else if (output is IEnumerable<INode> nodes)
                {
                    foreach (var _node in nodes)
                    {
                        dictionary[key].Nodes.Add(_node);
                        dictionaryMethodNameKeys[key] = _node.Data.ToString();
                        //_node.Name = key;
                    }
                }
                else if (output is IObservable<INode> nodeObservable)
                {
                    return Obs.Create<INode>(obs =>
                    {
                        return nodeObservable.Subscribe(_node =>
                        {
                            dictionary[key].Nodes.Add(_node);
                            dictionaryMethodNameKeys[key] = _node.Data.ToString(); ;
                            //_node.Name = key;
                            Add(_node);
                            obs.OnNext(_node);
                        });
                    });


                }
                else if (output is Task task)
                {
                    if (dictionary[key].Task == null)
                    {
                        dictionary[key].Task = task;

                    }
                    return Obs.Create<Node>(o =>
                    {
                        return dictionary[key].Task.ToObservable().Subscribe(a =>
                        {
                            var result = (Node)dictionary[key].Task.GetType().GetProperty("Result").GetValue(dictionary[key].Task);
                            dictionary[key].Nodes.Add(result);
                            Add(result);
                            o.OnNext(result);
                        });

                    });
                }
            }
            return Obs.Create<INode>(obs =>
            {
                foreach (var x in dictionary[key].Nodes)
                {
                    Add(x);
                    obs.OnNext(x);
                }
                return Disposable.Empty;
            });
        }
    }

    public class MethodValue
    {
        public MethodInfo Method { get; set; }

        public Task Task { get; set; }

        public IList<INode> Nodes { get; set; } = [];
    }
}
