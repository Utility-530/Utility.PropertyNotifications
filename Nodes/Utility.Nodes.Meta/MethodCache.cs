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

    public partial class NodesStore : INodeSource
    {
        private Lazy<IOP> lazyIOP = new(() =>
        {
            var iop = new IOP();
            var factories = Locator.Current.GetServices<IEnumerableFactory<Method>>();
            factories.ForEach(t => t.Create(null).ForEach(m => iop.Add(m.Name, t, m)));
            return iop;
        });

        public bool KeyExistsInCode(string key)
        {
            return lazyIOP.Value.Contains(key);
        }

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
                            foreach (var x in lazyIOP.Value._methods.Values)
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

                    if (lazyIOP.Value.Contains(key))
                    {
                        if (lazyIOP.Value[key].IsAccessed == false)
                        {
                            lazyIOP.Value[key].Nodes ??= [];
                            object output = lazyIOP.Value.Invoke(key);
                            if (output is NodeViewModel node)
                            {
                                lazyIOP.Value[key].Nodes.Add(node);
                                if (node.Name != null)
                                    throw new Exception($"Name gets assigned here ({nameof(NodesStore)}) automatically for top level nodes!");
                                node.Name = key;
                                observer.OnNext(node);
                                observer.OnCompleted();
                                return System.Reactive.Disposables.Disposable.Empty;
                           
                            }
                            else if (output is IEnumerable<INodeViewModel> nodes)
                            {
                                foreach (var _node in nodes)
                                {
                                    lazyIOP.Value[key].Nodes.Add(_node);
                                    observer.OnNext(_node);
                                }
                                observer.OnCompleted();
                                return System.Reactive.Disposables.Disposable.Empty;
                            }
                            else if (output is System.IObservable<INodeViewModel> nodeObservable)
                            {

                                return nodeObservable.Subscribe(_node =>
                                {
                                    lazyIOP.Value[key].Nodes.Add(_node);

                                    //_node.Name = key;
                                    //Add(_node);
                                    observer.OnNext(_node);
                                }, () => observer.OnCompleted());

                            }
                            else if (output is Task task)
                            {
                                if (lazyIOP.Value[key].Task == null)
                                {
                                    lazyIOP.Value[key].Task = task;
                                }

                                return lazyIOP.Value[key].Task.ToObservable().Subscribe(a =>
                                {
                                    var result = (INodeViewModel)lazyIOP.Value[key].Task.GetType().GetProperty("Result").GetValue(lazyIOP.Value[key].Task);
                                    lazyIOP.Value[key].Nodes.Add(result);
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
                            foreach (var x in lazyIOP.Value[key].Nodes)
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
                        throw new Exception($"potentially missing registration of {nameof(EnumerableMethodFactory)}");
                    }

                });
            }
        }

        public bool Contains(string key, Guid? guid)
        {
            return lazyIOP.Value[key].IsAccessed;
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