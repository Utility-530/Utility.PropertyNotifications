using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using Splat;
using Utility.Extensions;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Services.Meta;
using Observable = System.Reactive.Linq.Observable;

namespace Utility.Nodes.Meta
{
    public class IOP
    {
        public Dictionary<string, MethodValue> _methods = new();

        public void Add(string key, object instance, Method value)
        {
            _methods.Add(key, new MethodValue() { Instance = instance, Method = value });
        }

        public MethodValue this[string key] => _methods[key];
        public bool Contains(string key) => _methods.ContainsKey(key);

        public object Invoke(string key, object[]? objects = null)
        {
            if (_methods.ContainsKey(key))
                return this[key].Method.ExecuteWithObjects([.. objects ?? []]);
            throw new Exception("eeee 0ff0s");
        }
    }

    public partial class NodesStore : INodeSource, IObservableIndex<INodeViewModel>
    {
        private Lazy<IOP> dict = new(() =>
        {
            var iop = new IOP();
            var factories = Locator.Current.GetServices<IEnumerableFactory<Method>>();
            factories.ForEach(t => t.Create(null).ForEach(m => iop.Add(m.Name, t, m)));
            return iop;
        });

        public System.IObservable<INodeViewModel> this[string key]
        {
            get
            {
                return Observable.Create<INodeViewModel>(observer =>
                {
                    if (Guid.TryParse(key, out Guid guid))
                    {
                        if (this.Find(key) is not { } child)
                        {
                            foreach (var x in dict.Value._methods.Values)
                            {
                                if (x.Guid == guid)
                                {
                                    return this[x.Method.Name].Subscribe(observer);
                                }
                            }
                        }
                        else
                        {
                            observer.OnNext(child);
                            observer.OnCompleted();
                            return System.Reactive.Disposables.Disposable.Empty;
                        }

                        return many(key).Subscribe(node =>
                        {
                            observer.OnNext(node);
                        }, () => observer.OnCompleted());
                    }

                    if (dict.Value.Contains(key))
                    {
                        if (dict.Value[key].IsAccessed == false)
                        {
                            dict.Value[key].Nodes ??= [];
                            object output = dict.Value.Invoke(key);
                            if (output is NodeViewModel node)
                            {
                                dict.Value[key].Nodes.Add(node);
                                observer.OnNext(node);
                                observer.OnCompleted();
                                return System.Reactive.Disposables.Disposable.Empty;
                                //node.Name = key;
                            }
                            else if (output is IEnumerable<INodeViewModel> nodes)
                            {
                                foreach (var _node in nodes)
                                {
                                    dict.Value[key].Nodes.Add(_node);
                                    observer.OnNext(_node);
                                }
                                observer.OnCompleted();
                                return System.Reactive.Disposables.Disposable.Empty;
                            }
                            else if (output is System.IObservable<INodeViewModel> nodeObservable)
                            {

                                return nodeObservable.Subscribe(_node =>
                                {
                                    dict.Value[key].Nodes.Add(_node);

                                    //_node.Name = key;
                                    //Add(_node);
                                    observer.OnNext(_node);
                                }, () => observer.OnCompleted());

                            }
                            else if (output is Task task)
                            {
                                if (dict.Value[key].Task == null)
                                {
                                    dict.Value[key].Task = task;
                                }

                                return dict.Value[key].Task.ToObservable().Subscribe(a =>
                                {
                                    var result = (INodeViewModel)dict.Value[key].Task.GetType().GetProperty("Result").GetValue(dict.Value[key].Task);
                                    dict.Value[key].Nodes.Add(result);
                                    //Add(result);
                                    observer.OnNext(result);
                                }, () => observer.OnCompleted());

                            }
                            else
                            {
                                observer.OnNext(null);
                                observer.OnCompleted();
                                return System.Reactive.Disposables.Disposable.Empty;
                                //return Observable.Return<INodeViewModel>(null);
                            }
                        }
                        else
                        {
                            foreach (var x in dict.Value[key].Nodes)
                            {
                                //Add(x);
                                observer.OnNext(x);
                            }
                            observer.OnCompleted();
                            return System.Reactive.Disposables.Disposable.Empty;
                        }
                    }
                    else
                    {
                        throw new Exception("fr343 df");
                    }

                });
            }
        }

        public bool Contains(string key, Guid? guid)
        {
            return dict.Value[key].IsAccessed;
        }
    }

    public class MethodValue
    {
        public Guid Guid { get; set; }
        public bool IsAccessed => Task != null || Nodes != null;
        public object Instance { get; set; }
        public Method Method { get; set; }
        public Task Task { get; set; }
        public IList<INodeViewModel> Nodes { get; set; }
    }
}