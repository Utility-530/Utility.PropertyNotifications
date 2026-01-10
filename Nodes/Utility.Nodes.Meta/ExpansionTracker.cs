using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Utility.Changes;
using Utility.Enums;
using Utility.Extensions;
using Utility.Helpers;
using Utility.Helpers.Generic;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Ex;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.ServiceLocation;
using Utility.Services.Meta;
using Utility.Structs.Repos;
using CompositeDisposable = Utility.Observables.CompositeDisposable;
using Key = Utility.Structs.Repos.Key;

namespace Utility.Nodes.Meta
{
    public partial class NodeEngine
    {


        class State : NotifyPropertyClass
        {
            private Progress value;

            public Progress Value { get => value; set => this.RaisePropertyChanged(ref this.value, value); }
        }

        private const string ERROR_KEY_NOT_FOUND = "Key not found";
        private readonly Dictionary<string, List<INodeViewModel>> dictionary = new();
        private readonly IDataActivator dataActivator;
        Dictionary<string, State> states = new();
        Dictionary<string, CompositeDisposable> _childrenSubscriptions = new();
        private IDisposable unLoadDisposable;

        void setupExpansionHandling(INodeViewModel node)
        {
            states[node.Key()] = new State { Value = Progress.UnStarted };
            node.WhenReceivedFrom(n => n.IsExpanded)
                .DistinctUntilChanged()
                .SelectMany(isExpanded =>
                {
                    unLoadDisposable?.Dispose();
                    if (isExpanded)
                    {
                        dictionary.Get(node.Key());
                        if (states[node.Key()].Value == Progress.UnStarted)
                        {
                            return loadChildren(node);
                        }
                        else if (states[node.Key()].Value == Progress.Finished)
                        {
                            return reloadChildren(node);
                        }
                    }
                    else
                    {
                        return unloadChildren(node);
                    }
                    return Observable.Empty<Change<INodeViewModel>>();
                })
                .ObserveOn(Globals.UI)
                .Subscribe(child =>
                {
                    switch (child.Type)
                    {
                        case Changes.Type.Add:
                            node.Add(child.Value);
                            break;
                        case Changes.Type.Remove:
                            node.Remove(child.Value);
                            break;
                        case Changes.Type.Reset:
                            node.Clear();
                            break;
                        default:
                            throw new Exception("Unsupported change type");

                    }
                })
                .DisposeWith(childrenSubscriptions.Get(node.Key(), () => new CompositeDisposable()));

            IObservable<Change<INodeViewModel>> unloadChildren(INodeViewModel node)
            {
                return Observable.Create<Change<INodeViewModel>>(observer =>
                {
                    var value = states[node.Key()].Value;
                    if (value == Progress.Finished)
                        observer.OnNext(Change.Reset<INodeViewModel>());
                    else if (value == Progress.UnStarted)
                    {
                        // do nothing
                    }
                    else
                    {
                        unLoadDisposable = states[node.Key()].WithChangesTo(a => a.Value)
                        .Where(a => a == Progress.Finished)
                        .Take(1)
                        .Subscribe(a =>
                        {
                            Change.Reset<INodeViewModel>();
                            unLoadDisposable?.Dispose();
                        });
                        return unLoadDisposable;
                    }
                    return Disposable.Empty;
                });
            }

            IObservable<Change<INodeViewModel>> reloadChildren(INodeViewModel node)
            {
                return Observable.Create<Change<INodeViewModel>>(observer =>
                {
                    if (dictionary.ContainsKey(node.Key()) == false)
                    {
                        throw new Exception("dfg434 cvd44");
                    }
                    else
                    {
                        observer.OnNext(Change.Reset<INodeViewModel>());
                        foreach (INodeViewModel child in dictionary[node.Key()])
                        {
                            observer.OnNext(Change.Add<INodeViewModel>(child));
                        }
                        observer.OnCompleted();
                    }
                    return Disposable.Empty;
                });
            }

            IObservable<Change<INodeViewModel>> loadChildren(INodeViewModel node)
            {
                states[node.Key()].Value = Progress.InDeterminate;
                int childIndex = 0;
                return Observable.Create<Change<INodeViewModel>>(observer =>
                {
                    CompositeDisposable disposables = new();

                    if (node is IYieldItems yieldItems)
                    {
                        processYieldItems(yieldItems, observer).DisposeWith(disposables);
                         
                    }
                    if (node.IsProliferable)
                    {
                        states[node.Key()].Value = Progress.HalfWay;
                        return repository
                                    .Find((GuidKey)node.Key())
                                    .Take(1)
                                    .Subscribe(
                                        set =>
                                        {
                                            processKey(node, set, observer);
                                        },
                                        observer.OnError,
                                        () => { }
                                        ).DisposeWith(disposables);
                    }

                    return disposables;
                });

                void processKey(INodeViewModel node, Set<Structs.Repos.Key> set, IObserver<Change<INodeViewModel>> observer)
                {
                    int i = 0;
                    foreach (var change in set)
                    {
                        i++;
                        if (change is { Type: Changes.Type.None })
                        {
                            end();
                        }
                        else if (change is not { Type: Changes.Type.Add, Value: var key })
                            throw new Exception("V33D d3D");
                        else
                        {
                            if (node.IsSingular == false)
                            {
                                var child = nodesStore.Find(key.Guid.ToString());

                                if (child != null)
                                {
                                    if (child.IsSingular == false)
                                    {
                                        throw new Exception("dddf33s xcc");
                                    }
                                }
                                else
                                {
                                    findInRepository(node, key.Guid)
                                        .Where(a => a.IsSingular == false)
                                        .Select(a => Change.Add<INodeViewModel>(a))
                                        .Subscribe(observer.OnNext, () =>
                                        {
                                            end();
                                        });
                                }
                            }
                            else
                            {
                                //"no child nodes created yet";
                                end();
                            }
                        }
                    }

                    void end()
                    {
                        if (i == set.Count)
                        {
                            states[node.Key()].Value = Progress.Finished;
                            observer.OnCompleted();
                        }
                    }
                    //throw new Exception("DSF 33443");
                }

                IDisposable processYieldItems(IYieldItems yieldItems, IObserver<Change<INodeViewModel>> observer)
                {
                    CompositeDisposable composite = new();

                    var items = yieldItems
                        .Items()
                        .Cast<INodeViewModel>().ToArray();
                    if (items.Length > 0)
                        items.ForEach(child => createChildNode(node, child, ++childIndex, observer).DisposeWith(composite));
                    else if (states[node.Key()].Value != Progress.HalfWay)
                    {
                        states[node.Key()].Value = Progress.Finished;
                    }
                    if (yieldItems is not INotifyCollectionChanged collectionChanged)
                    {
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }
                    CompositeDisposable? disposable = null;
                    return yieldItems
                        .Items()
                        .Changes<INodeViewModel>()
                        .Subscribe(set =>
                        {
                            disposable?.Dispose();
                            disposable = new();
                            foreach (var item in set)
                                switch (item.Type)
                                {
                                    case Changes.Type.Add:
                                        createChildNode(node, item.Value, ++childIndex, observer).DisposeWith(disposable);
                                        break;
                                    default:
                                        throw new Exception("Unsupported change type");
                                }
                        });

                  
                    IDisposable createChildNode(INodeViewModel parent, INodeViewModel child, int index, IObserver<Changes.Change<INodeViewModel>> observer)
                    {
                        if (child.Name() == null)
                            throw new Exception("child name is null");

                        return repository
                            .Find((GuidKey)parent.Key(), child.Name(), type: GetNodeType(child), index: index)
                            .Take(1)
                            .Subscribe(set =>
                            {
                                foreach (var change in set)
                                {

                                    if (change.Type != Changes.Type.Add)
                                        throw new Exception("V33D");
                                    //if (change.Value == false)
                                    //    throw new Exception("Key is null");
                                    keys.Add(change.Value);

                                    var existingNode = nodesStore.Find(change.Value.Guid.ToString());
                                    if (existingNode != null)
                                    {
                                        // if current value set then already created
                                    }
                                    else
                                    {
                                        validateKey(change.Value);
                                        child.SetKey(new GuidKey(change.Value.Guid));
                                        dictionary.Get(parent.Key()).Add(child);
                                        observer.OnNext(Change.Add<INodeViewModel>(child));
                                    }
                                    if (items.Length == index && states[node.Key()].Value != Progress.HalfWay)
                                    {
                                        states[node.Key()].Value = Progress.Finished;
                                    }
                                }
                            });
                    }
                }
            }
        }

        public IObservable<INodeViewModel> findInRepository(INodeViewModel node, Guid guid)
        {
            return repository
            .Find((GuidKey)node.Key(), guid: guid)
            .Take(1)
            .Select(set =>
            {
                if (set.Single() is not { Type: Utility.Changes.Type.Add, Value: Key value })
                    throw new Exception("V33d d3D");
                return activateNode(node, value);
            });

            INodeViewModel activateNode(INodeViewModel parent, Structs.Repos.Key? key)
            {
                if (key.HasValue == false)
                    throw new Exception("Key is null");
                keys.Add(key.Value);
                validateKey(key);

                var newNode = (INodeViewModel)dataActivator.Activate(key);
                newNode.SetParent(parent);
                newNode.SetKey(new GuidKey(key.Value.Guid));
                newNode.Removed = key.Value.Removed;
                dictionary.Get(parent.Key()).Add(newNode);
                return newNode;
            }

        }

        public IObservable<INodeViewModel> FindChild(INodeViewModel node, Guid guid)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                var child = nodesStore.Find(guid.ToString());
                {
                    if (child != null)
                    {
                        add(child);
                        return Disposable.Empty;
                    }
                    else
                    {
                        // Search in repository
                        return
                        findInRepository(node, guid)
                        .Subscribe(activatedNode =>
                        {
                            observer.OnNext(activatedNode);
                            observer.OnCompleted();
                        });
                    }
                }

            });
        }


        public IObservable<INodeViewModel> Roots()
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                int i = 0;
                return
                repository
                .Find()
                .Take(1)
                .Subscribe(set =>
                {
                    foreach (var change in set)
                    {
                        if (change.Type != Changes.Type.Add)
                            throw new Exception("V33D");
                        var key = change.Value;

                        this.keys.Add(key);
                        this.roots.Add(key);
                        var existingNode = nodesStore.Find(key.Guid.ToString());
                        if (existingNode == null)
                        {
                            i++;
                            if (this.nodesStore.KeyExistsInCode(key.Name))
                                Create(key.Name).Subscribe(a =>
                                {
                                    if (a != null)
                                    {
                                        // if current value set then already created
                                    }
                                    else if (key.Removed.HasValue)
                                    {

                                    }
                                    else
                                    {
                                        observer.OnNext(create(key));
                                    }
                                    observer.OnNext(a);
                                }, () => { i--; if (i == 0) observer.OnCompleted(); });
                            else if (key.Removed.HasValue)
                            {
                                i--;
                            }
                            else
                            {
                                observer.OnNext(create(key));
                                i--;
                            }
                        }
                        else
                        {
                            observer.OnNext(existingNode);
                            i--;
                        }
                    }
                }, () => { if (i == 0) observer.OnCompleted(); });
            });

            INodeViewModel create(Key key)
            {
                var child = (INodeViewModel)dataActivator.Activate(key);
                validateKey(key);
                child.SetKey(new GuidKey(key.Guid));
                child.IsProliferable = true;
                add(child);
                return child;
            }
        }

    }
}
