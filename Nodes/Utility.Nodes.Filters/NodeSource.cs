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

namespace Utility.Nodes.Filters
{
    public class NodeSource
    {
        public static string New = "new";
        public static readonly string Key = nameof(NodeSource);

        private Dictionary<string, MethodValue> dictionary;
        private Dictionary<string, string> dictionaryMethodNameKeys = [];

        private NodeSource()
        {
            Nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        private void Nodes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Node node in e.NewItems)
                {
                    if(node.Key== "1c6e643b-2f8d-486d-aee2-e49144383728")
                    {

                    }
                    if (Nodes.Count(x => x.Key == node.Key) > 1)
                    {

                    }
                }
        }

        public ObservableCollection<Node> Nodes { get; } = new();

        public static NodeSource Instance { get; } = new();

        public IObservable<Node?> Single(string key)
        {
            return Many(key).Take(1);
        }

        public IObservable<Node> SingleByNameAsync(string name)
        {
            return Nodes.SelfAndAdditions().Where(a => a.Name == name).Take(1);
        }

        public IObservable<Node> ChildrenByGuidAsync(Guid guid)
        {

            return Observable.Create<Node>(observer =>
            {
                try
                {
                    return TreeRepository.Instance.Find(guid)
                        .Subscribe(a =>
                        {
                            if (a.Guid == default)
                            {

                            }
                            SingleByGuidAsync(a.Guid)
                            .Subscribe(_a =>
                            {
                                _a.Name ??= a.Name;
                                observer.OnNext(_a);
                            }
                            );
                        }, () => observer.OnCompleted());
                }
                catch (Exception ex) when (ex.Message == TreeRepository.No_Existing_Table_No_Name_To_Create_New_One)
                {
                    var node = new Node(New, null);
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


        public IObservable<Node> SingleByGuidAsync(Guid guid)
        {
            return Observable.Create<Node>(observer =>
            {
                if (Nodes.SingleOrDefault(a => a.Key == new GuidKey(guid)) is not Node node)
                {
                    return TreeRepository.Instance.Get(guid)
                            .Subscribe(_d =>
                            {

                                var value = _d.Value;
                                try
                                {
                                    var _node = Splat.Locator.Current.GetService<IMapper>()?.Map<NodeDTO, Node>((NodeDTO)value);
                                    if (_node != null)
                                    {
                                        observer.OnNext(_node);
                                        observer.OnCompleted();
                                        //_node.Parent = Nodes.Single(a => a.Guid == guid);
                                    }
                                    else
                                    {

                                        var node = new Node(null, null) { Key = new GuidKey(guid) };
                                        observer.OnNext(node);
                                        observer.OnCompleted();
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
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
                    dictionaryMethodNameKeys[key] = node.Name;
                    //node.Name = key;
                }
                else if (output is IEnumerable<Node> nodes)
                {
                    foreach (var _node in nodes)
                    {
                        dictionary[key].Nodes.Add(_node);
                        dictionaryMethodNameKeys[key] = _node.Name;
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
                            dictionaryMethodNameKeys[key] = _node.Name;
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

        public IObservable<Node> FindNodeAsync(Guid currentGuid)
        {
            return Observable.Create<Node>(observer =>
            {
                return Nodes.SelfAndAdditions().Subscribe(a =>
                {
                    if (a.Key == new GuidKey(currentGuid))
                        observer.OnNext(a);
                });
            });
        }
    }

    public class MethodValue
    {
        public MethodInfo Method { get; set; }

        public Task Task { get; set; }

        public IList<Node> Nodes { get; set; } = new List<Node>();
    }
}
