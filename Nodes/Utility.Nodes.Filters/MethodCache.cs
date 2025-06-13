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

namespace Utility.Nodes.Filters
{
    public class MethodsValue(object instance, MethodInfo methodInfo)
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

        public void Add(string key, object instance, MethodInfo value)
        {
            _methods.Add(key, new(instance, value));
        }

        public MethodValue this[(string key, Guid? guid) yt] => _methods[yt.key][yt.guid ?? default];

        public object Invoke(string key, Guid? guid, object[]? objects = null)
        {
            if (_methods.ContainsKey(key))
                return this[(key, guid)].Method.Invoke(this[(key, guid)].Instance, (guid.HasValue ? [guid.Value] : Array.Empty<object>()).Concat(objects != null? objects: Array.Empty<object>()).ToArray());
            throw new Exception("eeee 0ff0s");
        }
    }

    public class MethodCache
    {
        private Lazy<IOP> dict = new(() =>
        {
            var iop = new IOP();
            var factories = Locator.Current.GetServices<INodeMethodFactory>();
            factories.ForEach(t => t.Methods.ForEach(m => iop.Add(m.Name, t, m)));
            return iop;
        });

        public MethodCache()
        {

        }

        private MethodValue this[(string key, Guid? guid) x]
        {
            get
            {
                return dict.Value[x];
            }
        }


        public IObservable<INode> Get(string key, Guid? guid = default, object?[] objects = null)
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
                else if (output is IObservable<INode> nodeObservable)
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

    }
}