using ActivateAnything;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Observable = System.Reactive.Linq.Observable;
using Utility.Helpers;
using System.Reflection;
using Utility.Interfaces.Generic;
using Utility.Models;

namespace Utility.Nodes.Filters
{
    public class MethodsValue(object instance, Method methodInfo)
    {
        Dictionary<Guid, MethodValue> pairs = new();

        public MethodValue this[Guid guid]
        {
            get
            {
                return pairs.Get(guid, a => new MethodValue() { Instance = instance, Method = methodInfo });
            }
        }
    }

    public class IOP
    {
        Dictionary<string, MethodsValue> _methods = new();

        public void Add(string key, object instance, Method value)
        {
            _methods.Add(key, new(instance, value));
        }

        public MethodValue this[(string key, Guid? guid) yt] => _methods[yt.key][yt.guid ?? default];

        public object Invoke(string key, Guid? guid, object[]? objects = null)
        {
            if (_methods.ContainsKey(key))
                return this[(key, guid)].Method.Execute([.. (guid.HasValue ? [guid.Value] : Array.Empty<object>()), .. objects ?? Array.Empty<object>()]);
            throw new Exception("eeee 0ff0s");
        }
    }

    public class MethodCache : IObservableIndex<INode>
    {
        private Lazy<IOP> dict = new(() =>
        {
            var iop = new IOP();
            var factories = Locator.Current.GetServices<IEnumerableFactory<Method>>();
            factories.ForEach(t => t.Create(null).ForEach(m => iop.Add(m.Name, t, m)));
            return iop;
        });

        private MethodCache()
        {

        }

        private MethodValue this[(string key, Guid? guid) x]
        {
            get
            {
                return dict.Value[x];
            }
        }


        public System.IObservable<INode> this[string key] => Get(key);

        public System.IObservable<INode> Get(string key, Guid? guid = default, object?[] objects = null)
        {
            if (Contains(key, guid) == false)
            {
                this[(key, guid)].Nodes ??= [];
                object output = dict.Value.Invoke(key, guid, objects);
                if (output is Node node)
                {
                    this[(key, guid)].Nodes.Add(node);

                    //node.Name = key;
                }
                else if (output is IEnumerable<INode> nodes)
                {
                    foreach (var _node in nodes)
                    {
                        this[(key, guid)].Nodes.Add(_node);

                        //_node.Name = key;
                    }
                }
                else if (output is System.IObservable<INode> nodeObservable)
                {
                    return Observable.Create<INode>(obs =>
                    {
                        return nodeObservable.Subscribe(_node =>
                        {
                            this[(key, guid)].Nodes.Add(_node);

                            //_node.Name = key;
                            //Add(_node);
                            obs.OnNext(_node);
                        });
                    });
                }
                else if (output is Task task)
                {
                    if (this[(key, guid)].Task == null)
                    {
                        this[(key, guid)].Task = task;
                    }
                    return Observable.Create<Node>(o =>
                    {
                        return this[(key, guid)].Task.ToObservable().Subscribe(a =>
                        {
                            var result = (Node)this[(key, guid)].Task.GetType().GetProperty("Result").GetValue(this[(key, guid)].Task);
                            this[(key, guid)].Nodes.Add(result);
                            //Add(result);
                            o.OnNext(result);
                        });
                    });
                }
                else
                {
                    return Observable.Return<INode>(null);
                }
            }
            return Observable.Create<INode>(obs =>
            {
                foreach (var x in this[(key, guid)].Nodes)
                {
                    //Add(x);
                    obs.OnNext(x);
                }
                return System.Reactive.Disposables.Disposable.Empty;
            });
        }

        public bool Contains(string key, Guid? guid)
        {
            return this[(key, guid)].IsAccessed;
        }


        public static MethodCache Instance { get; } = new();
    }
}