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
using Utility.Helpers.NonGeneric;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Utility.Nodes.Ex;
using Utility.Extensions;
using Utility.Models.Trees;
using Utility.Trees.Abstractions;
using Utility.Models;
using Fasterflect;
using System.ComponentModel;
using System.Reactive.Disposables;
using CompositeDisposable = Utility.Observables.CompositeDisposable;

namespace Utility.Nodes.Filters
{
    public readonly record struct DirtyNode(string Property, INode Node);
    public class NodeEngine : INodeSource
    {
        public static string New = "new";
        public static readonly string Key = nameof(NodeEngine);

        Lazy<IFilter> filter;
        Lazy<IExpander> expander;
        Lazy<IContext> context;
        Lazy<ITreeRepository> repository;
        Lazy<MethodCache> methodCache;

        CompositeDisposable compositeDisposable = new();

        private Lazy<Dictionary<string, Setter>> setdictionary = new(() => typeof(Node)
                                                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                    .Where(a => a.Name != nameof(IReadOnlyTree.Parent))
                                                    .ToDictionary(a => a.Name, a => Rules.Decide(a)));
        private bool _disposed;
        private readonly ObservableCollection<INode> nodes = [];

        public NodeEngine()
        {
            filter = new(() => Locator.Current.GetService<IFilter>());
            expander = new(() => Locator.Current.GetService<IExpander>());
            context = new(() => Locator.Current.GetService<IContext>());
            var repdository = Locator.Current.GetService<ITreeRepository>();
            repository = new(() => repdository);
            methodCache = new(() => Locator.Current.GetService<MethodCache>());
        }

        public Guid Guid { get; } = Guid.NewGuid();

        public IReadOnlyCollection<INode> Nodes => nodes;

        public static NodeEngine Instance { get; } = new();

        string INodeSource.New => New;


        public void Remove(INode node)
        {
            nodes.Remove(node);
            repository.Value.Remove((GuidKey)node.Key);
        }

        public void Add(INode node)
        {
            if (node.Data is IIgnore)
                return;
            if (node.Key is null)
            {
                var index = node.Parent.Items.Count(a => ((INode)a).Name() == node.Name());
                var type =
                repository
                    .Value
                    .Find((GuidKey)node.Parent.Key, node.Name(), type: toType(node.Data), index: index == 0 ? null : index)
                    .Subscribe(_key =>
                    {
                        if (_key.HasValue == false)
                        {
                            throw new Exception("dde33443 21");
                        }

                        node.Key = new GuidKey(_key.Value.Guid);
                        Add(node);
                    });
                return;
            }
            else
            {
                nodes.Add(node);
                configure(node);

                node
                    .WithChangesTo(a => a.Data)
                    .Where(a => a is not string)
                    .Take(1)
                    .Subscribe(data =>
                    {
                        loadProperties(node).Subscribe(a =>
                        {
                            initialiseData(a.Data);


                            node.WithChangesTo(a => a.IsExpanded)
                            .Where(a => a == true)
                            .Take(1)
                            .Subscribe(a =>
                            {
                                if (data is IYieldChildren ychildren)
                                    _children(node, ychildren).Subscribe(a =>
                                    {
                                        node.Add(a);
                                    });
                            }).DisposeWith(compositeDisposable);
                        }).DisposeWith(compositeDisposable);
                    }).DisposeWith(compositeDisposable);

                node.Items.AndChanges<INode>().Subscribe(a =>
                {
                    foreach (var item in a)
                        change(item);
                }).DisposeWith(compositeDisposable);
            }


            void configure(INode node)
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
                string[] names = [nameof(Node.IsSelected), nameof(Node.IsExpanded), nameof(Node.Orientation), nameof(Node.Order), nameof(Node.Arrangement), nameof(Node.Order)];
                node.WithChanges().Subscribe(e =>
                {
                    if (e is PropertyChangedExEventArgs { PreviousValue: var previousValue, PropertyName: string name, Value: var value })
                    {
                        if (name == nameof(ViewModelTree.Order))
                        {
                            ((value as ITree).Parent as INode).Sort(null);
                        }
                        if (names.Contains(name))
                        {
                            repository.Value.Set((GuidKey)node.Key, name, value, DateTime.Now);
                        }
                        else
                            context.Value.UI(() =>
                            {
                                Single(nameof(Factory.BuildDirty))
                                .Subscribe(async _node =>
                                {
                                    if (_node.Data is not CollectionModel<DirtyModel> { Collection: { } collection } exceptionsModel)
                                        throw new Exception("775 333");
                                    else
                                        _node.Add(await _node.ToTree(new DirtyModel { Name = name + node.Items.Count(), SourceKey = node.Key, PropertyName = name, NewValue = value }));
                                }).DisposeWith(compositeDisposable);
                            });
                    }
                    else
                        throw new Exception("ss FGre333333333");
                });
            }


            void initialiseData(object data)
            {


                if (data is INotifyPropertyCalled called)
                {
                    called.WhenCalled()
                    .Subscribe(call =>
                    {
                        if (call.Name == nameof(Model.Name))
                        {
                            return;
                        }
                        repository.Value
                        .Get((GuidKey)node.Key, nameof(Node.Data) + "." + call.Name)
                        .Subscribe(a =>
                        {
                            if (a.Value != null)
                            {
                                object output = null;
                                FieldInfo fieldInfo = null;

                                if (call.Target is IGet get)
                                {
                                    output = get.Get();
                                }
                                else if (call.Target.TryGetPrivateFieldValue(call.Name.ToLower(), out var _output, out var _fieldInfo) == false)
                                {
                                    output = _output;
                                    fieldInfo = _fieldInfo;
                                }
                                else
                                {
                                    throw new Exception($"no field for property, {call.Name}");
                                }


                                if (output?.Equals(a.Value) == true)
                                    return;
                                else
                                {
                                    if (call.Target is ISet set)
                                    {
                                        set.Set(a.Value);
                                    }
                                    else if (fieldInfo != null)
                                    {
                                        fieldInfo.SetValue(call.Target, a.Value);
                                    }
                                    else
                                    {
                                        throw new Exception($"no field for property, {call.Name}");
                                    }
                                    if (call.Target is INotifyPropertyChanged changed)
                                        changed.RaisePropertyChanged(call.Name);
                                }
                            }
                        }).DisposeWith(compositeDisposable);
                    }).DisposeWith(compositeDisposable);
                }
                if (data is INotifyPropertyReceived received)
                {
                    received.WhenReceived()
                    .Subscribe(reception =>
                    {
                        if (reception.Name == nameof(Model.Name))
                        {
                            (reception.Target as INotifyPropertyChanged).RaisePropertyChanged(reception.Name);
                            repository.Value.UpdateName((GuidKey)node.Parent.Key, (GuidKey)node.Key, (string)reception.OldValue, (string)reception.Value);
                        }
                        else
                            repository.Value.Set((GuidKey)node.Key, nameof(Node.Data) + "." + reception.Name, reception.Value, DateTime.Now);
                    }).DisposeWith(compositeDisposable);
                }


                if (data is ISetNode iSetNode)
                {
                    iSetNode.SetNode(node);
                }
                else
                {
                    //throw new Exception("R333 ");
                }


                if (data is IValue value && value.Value != null)
                    repository.Value.Set((GuidKey)node.Key, nameof(Node.Data) + "." + nameof(IValue.Value), value.Value, DateTime.Now);

            }

            IObservable<INode> loadProperties(INode node)
            {
                return Observable.Create<INode>(observer =>
                {
                    return repository.Value.Get(Guid.Parse(node.Key)).Subscribe(_d =>
                    {
                        if (_d.Name == null)
                        {
                        }
                        else if (_d.Value != null && setdictionary.Value.TryGetValue(_d.Name, out Setter value))
                            value.Set(node, _d.Value);
                    }, () =>
                    {
                        observer.OnNext(node);
                        observer.OnCompleted();
                    });
                });
            }



            void change(Change a)
            {
                if (a is Change { Type: Changes.Type.Add, Value: { } value })
                {
                    if (value is INode _node)
                        Add(_node);
                    else
                    {
                        //node.Add(await node.ToTree(value));
                    }
                }
                else if (a is Change { Type: Changes.Type.Remove, Value: { } _value })
                {
                    if (_value is not INode node)
                    {
                        throw new Exception("  333 sdsdf");
                    }
                    nodes.RemoveBy<INode>(c =>

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

                    repository.Value.Remove((GuidKey)node.Key);
                }
                else if (a is Change { Type: Changes.Type.Update, Value: INode newValue, OldValue: INode oldValue })
                {
                    nodes.RemoveBy<INode>(c =>
                    {
                        if (c is IKey key)
                        {
                            if (oldValue is IKey _key)
                            {
                                return key.Key.Equals(oldValue.Key);
                            }
                            //else if (_value is IGetGuid guid)
                            //{
                            //    return key.Key.Equals(new GuidKey(guid.Guid));
                            //}
                        }
                        throw new Exception("44c dd");

                    });
                }
            }

            IObservable<INode> _children(INode node, IYieldChildren children)
            {
                return Observable.Create<INode>(observer =>
                {
                    int i = 0;

                    return repository.Value.Find((GuidKey)node.Key)
                        .Subscribe(async key =>
                        {
                            if (key.HasValue == false)
                            {
                                children
                                .Children
                                    .ForEach(async d =>
                                    {
                                        NewMethod(node, observer, d, ++i).DisposeWith(compositeDisposable);
                                    });
                            }
                            else
                            {
                                i++;
                                if (Nodes.SingleOrDefault(a => a.Key == new GuidKey(key.Value.Guid)) is not Node _node)
                                {
                                    var _new = (INode)await node.ToTree(DataActivator.Activate(key));
                                    _new.Key = new GuidKey(key.Value.Guid);
                                    _new.Removed = key.Value.Removed;
                                    observer.OnNext(_new);
                                }
                                else
                                {
                                    // child node expanded before parent
                                    return;
                                    throw new Exception("u 333333312");
                                }

                            }

                        }, () =>
                        {

                            children
                            .Children
                             .Additions()
                             .Subscribe(d =>
                             {
                                 NewMethod(node, observer, d, ++i).DisposeWith(compositeDisposable);

                             }).DisposeWith(compositeDisposable);

                        }
                            /*,() => observer.OnCompleted()*/);
                });

                IDisposable NewMethod(INode node, IObserver<INode> observer, object d, int i)
                {
                    return node.ToTree(d).ToObservable().Cast<INode>().Subscribe(_new =>
                    {
                        repository
                        .Value
                        .Find((GuidKey)node.Key, _new.Name(), type: toType(d), index: ++i)
                        .Subscribe(_key =>
                        {

                            if (d is IIsReadOnly readOnly)
                            {
                                (_new as ISetIsReadOnly).IsReadOnly = readOnly.IsReadOnly;
                            }

                            if (_key.HasValue == false)
                            {
                                throw new Exception("dde33443 21");
                            }
                            _new.Key = new GuidKey(_key.Value.Guid);

                            observer.OnNext(_new);
                        });
                    });

                }
            }
        }

        public void Save()
        {
            Single(nameof(Factory.BuildDirty))
                .Subscribe(async tree =>
                {
                    if (tree.Data is not CollectionModel<DirtyModel> model)
                        throw new Exception("DSF 64333");
                    foreach (var item in tree.ToArray())
                    {
                        if (item.Data is DirtyModel { SourceKey: { } sk, PropertyName: { } pn, NewValue: { } nv } dmodel)
                        {
                            //var node = item.Value.Source as INode;
                            repository.Value.Set(Guid.Parse(sk), pn, nv, DateTime.Now);
                            tree.Remove(item);
                            await Task.Delay(200);
                        }
                    }
                }).DisposeWith(compositeDisposable);
        }

        public IObservable<INode> Single(string key)
        {
            return Many(key).SelectMany(a => a.WithChangesTo(n => n.Data).Where(x => x is not string).Select(ax => a));
        }

        public IObservable<INode> Many(string key)
        {
            return Observable.Create<INode>(observer =>
            {
                if (Guid.TryParse(key, out var _key))
                    return Nodes
                    .AndAdditions<INode>()
                    .Subscribe(ax =>
                    {
                        if (ax.Key.Equals(_key))
                        {
                            observer.OnNext(ax);
                            observer.OnCompleted();
                        }
                    });
                else
                    return methodCache.Value
                    .Get(key)
                    .Subscribe(a =>
                    {
                        observer.OnNext(a);
                        //observer.OnCompleted();
                    }, () =>
                    {
                        observer.OnCompleted();
                    });
            });
        }

        public IObservable<INode> Create(string name, Guid guid, Func<string, INode> nodeFactory, Func<string, object> modelFactory)
        {
            return Observable.Create<INode>(observer =>
            {
                INode node;
                if (Nodes.SingleOrDefault(a => (GuidKey)a.Key == guid) is { } _node)
                {
                    lock (nodes)
                        observer.OnNext(_node);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                else
                {
                    node = nodeFactory(name);
                    node.Key = new GuidKey(guid);
                }
                observer.OnNext(node);

                var data = modelFactory(name);

                var disposable = repository.Value
                .InsertRoot(guid, name, toType(data))
                .Subscribe(a =>
                {
                    if (a.HasValue)
                    {
                        node.Data = data;
                    }
                    if (a.HasValue && node.Data == null)
                        node.Data = DataActivator.Activate(a);
                    Add(node);
                    observer.OnCompleted();
                });

                return disposable;
            });
        }



        System.Type toType(object data)
        {
            return data is IGetType { } getType ? getType.GetType() : data.GetType();
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        //The Dispose method performs all object cleanup, so the garbage collector no longer needs to call the objects' Object.Finalize override. Therefore, the call to the SuppressFinalize method prevents the garbage collector from running the finalizer. If the type has no finalizer, the call to GC.SuppressFinalize has no effect. The actual cleanup is performed by the Dispose(bool) method overload.

        //In the overload, the disposing parameter is a Boolean that indicates whether the method call comes from a Dispose method(its value is true) or from a finalizer(its value is false).


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                // ...
                compositeDisposable.Dispose();
                nodes.Clear();
            }

            // Free unmanaged resources.
            // ...

            _disposed = true;
        }
    }

    public class MethodValue
    {
        public MethodInfo Method { get; set; }

        public Task Task { get; set; }

        public IList<INode> Nodes { get; set; } = [];
    }
}
