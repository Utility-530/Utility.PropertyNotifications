using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Obs = System.Reactive.Linq.Observable;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Reactive.Disposables;
using Utility.Repos;
using AutoMapper;
using Splat;
using System.Threading.Tasks;
using System.Collections.Generic;
using Utility.Reactives;
using System.Linq;
using Utility.Keys;
using Utility.Structs.Repos;
using Utility.Interfaces.Exs;
using Utility.Changes;
using Utility.PropertyNotifications;
using Utility.Interfaces;
using Utility.Helpers;
using NetFabric.Hyperlinq;
using Utility.Helpers.Generic;

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
        private Lazy<Dictionary<string, Action<object, object>>> setdictionary = new(() => typeof(Node)
                                                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    .ToDictionary(a => a.Name, a => new Action<object, object>((instance, value) => a.SetValue(instance, value))));
        private ObservableCollection<INode> nodes = [];

        private NodeSource()
        {
        }



        public IReadOnlyCollection<INode> Nodes => nodes;
        public ObservableCollection<KeyValuePair<string, PropertyChange>> DirtyNodes => dirtyNodes;
        public static NodeSource Instance { get; } = new();
        string INodeSource.New => New;

        public IObservable<Node?> Single(string key)
        {
            return Many(key).Take(1);
        }

        public IObservable<INode> SingleByNameAsync(string name)
        {
            return this.nodes.SelfAndAdditions().Where(a => a.Data.ToString() == name).Take(1);
        }

        public IObservable<INode> ChildrenByGuidAsync(Guid guid)
        {
            return Observable.Create<INode>(observer =>
            {
                try
                {
                    return repository.Find(guid)
                        .Subscribe(a =>
                        {
                            if (a.Guid == default)
                            {

                            }
                            SingleByGuidAsync(a.Guid)
                            .Subscribe(_a =>
                            {
                                //_a.Name ??= a.Name;
                                _a.Data ??= a.Name;
                                _a.Removed = a.Removed;
                                observer.OnNext(_a);
                            }
                            );
                        }, () => observer.OnCompleted());
                }
                catch (Exception ex) when (ex.Message == TreeRepository.No_Existing_Table_No_Name_To_Create_New_One)
                {
                    var node = new Node(New);
                    observer.OnNext(node);
                    observer.OnCompleted();
                }
                return Disposable.Empty;
            });
        }

        public Node? SingleByGuid(Guid guid)
        {
            if (Nodes.SingleOrDefault(a => a.Key == guid.ToString()) is not Node node)
            {
                return null;
            }
            else
            {
                return (node);
            }
        }

        public IObservable<INode> SingleByGuidAsync(Guid guid)
        {
            return Observable.Create<Node>(observer =>
            {
                if (Nodes.SingleOrDefault(a => a.Key == new GuidKey(guid)) is not Node node)
                {
                    node = new Node("_New_") { Key = new GuidKey(guid) };

                    return repository.Get(guid)
                            .Subscribe(_d =>
                            {
                                if (_d.Value != null && setdictionary.Value.ContainsKey(_d.Name))
                                    setdictionary.Value[_d.Name].Invoke(node, _d.Value);
                            }, () =>
                        {
                            observer.OnNext(node);
                            observer.OnCompleted();
                        });
                }
                else
                {
                    observer.OnNext(node);
                    observer.OnCompleted();

                    return Disposable.Empty;
                }
            });
        }

        public IObservable<Node> Many(string key)
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
                else if (output is IEnumerable<Node> nodes)
                {
                    foreach (var _node in nodes)
                    {
                        dictionary[key].Nodes.Add(_node);
                        dictionaryMethodNameKeys[key] = _node.Data.ToString();
                        //_node.Name = key;
                    }
                }
                else if (output is IObservable<Node> nodeObservable)
                {
                    return Obs.Create<Node>(obs =>
                    {
                        return nodeObservable.Subscribe(_node =>
                        {
                            dictionary[key].Nodes.Add(_node);
                            dictionaryMethodNameKeys[key] = _node.Data.ToString(); ;
                            //_node.Name = key;
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
                            o.OnNext(result);
                        });

                    });
                }
            }
            return Obs.Create<Node>(obs =>
            {
                foreach (var x in dictionary[key].Nodes)
                {
                    obs.OnNext(x);
                }
                return Disposable.Empty;
            });
        }

        public IObservable<INode> FindNodeAsync(Guid currentGuid)
        {
            return Observable.Create<INode>(observer =>
            {
                return nodes.SelfAndAdditions().Subscribe(a =>
                {
                    if (a.Key == new GuidKey(currentGuid))
                        observer.OnNext(a);
                });
            });
        }

        public DateTime Remove(Guid guid)
        {
            var d = repository.Remove(guid);
            this.nodes.RemoveBy(a => (GuidKey)a.Key == guid);
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

        public IObservable<Structs.Repos.Key> Find(Guid guid, string name, System.Type type, int? localIndex)
        {
            return repository.Find(guid, name, type, localIndex);
        }

        public IObservable<DateValue> Get(Guid guid, string name)
        {
            return repository.Get(guid, name);
        }

        public void Add(INode node)
        {
            if (this.Nodes.Any(a => a.Key == node.Key) == false)
            {
                this.nodes.Add(node);
                node.PropertyChanged += Node_PropertyChanged;
                node.AndAdditions<INode>().Subscribe(Add);

                node.WithChangesTo(a => a.Key, false)
                    .Subscribe(xx =>
                    {
                        repository
                        .Get(Guid.Parse(node.Key), nameof(Node.Data))
                        .Subscribe(a =>
                        {
                            if (a.Value == null)
                            {
                                try
                                {
                                    repository.Set(Guid.Parse(node.Key), nameof(Node.Data), node.Data, DateTime.Now);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        });
                    });
                if (Nodes.Count(x => x.Key == node.Key) > 1)
                {

                }
            }

            void Node_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (sender is INode node)
                {
                    if (e is PropertyChangedExEventArgs { PreviousValue: var previousValue, PropertyName: string name, Value: var value })
                    {
                        dirtyNodes.Add(new(node.Key, new PropertyChange(sender, name, value, previousValue)));
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
    }

    public class MethodValue
    {
        public MethodInfo Method { get; set; }

        public Task Task { get; set; }

        public IList<Node> Nodes { get; set; } = new List<Node>();
    }
}
