using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Observable = System.Reactive.Linq.Observable;

namespace Utility.Nodes.Filters
{
    public class MethodCache
    {
        private Dictionary<string, MethodValue> dictionary;
        private Dictionary<string, string> dictionaryMethodNameKeys = [];

        private MethodCache()
        {
            dictionary ??= typeof(Factory)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .ToDictionary(a => a.Name, a => new MethodValue { Method = a });
        }

        public IObservable<INode> Get(string key)
        {
            if (Contains(key) == false)
            {
                object output = Invoke(key);
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
                    return Observable.Create<INode>(obs =>
                    {
                        return nodeObservable.Subscribe(_node =>
                        {
                            dictionary[key].Nodes.Add(_node);
                            dictionaryMethodNameKeys[key] = _node.Data.ToString(); ;
                            //_node.Name = key;
                            //Add(_node);
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
                    return Observable.Create<Node>(o =>
                    {
                        return dictionary[key].Task.ToObservable().Subscribe(a =>
                        {
                            var result = (Node)dictionary[key].Task.GetType().GetProperty("Result").GetValue(dictionary[key].Task);
                            dictionary[key].Nodes.Add(result);
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
                foreach (var x in dictionary[key].Nodes)
                {
                    //Add(x);
                    obs.OnNext(x);
                }
                return Disposable.Empty;
            });
        }

        public bool Contains(string key)
        {
            return dictionaryMethodNameKeys.ContainsKey(key);
        }

        public static MethodCache Instance { get; } = new();

        public object? Invoke(string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key].Method.Invoke(null, Array.Empty<object>());
            return null;
        }
    }
}