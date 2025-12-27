using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
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
        private const string ERROR_KEY_NOT_FOUND = "Key not found";
        private readonly Dictionary<INodeViewModel, List<INodeViewModel>> dictionary = new();
        private readonly IDataActivator dataActivator;
        private bool _disposed;

        void setupExpansionHandling(INodeViewModel node)
        {
            node.WhenReceivedFrom(n => n.IsExpanded)
                .DistinctUntilChanged()
                //.Where(isExpanded => isExpanded)
                .SelectMany(isEx =>
                {
                    if (isEx)
                    {
                        if (node.AreChildrenLoaded == false)
                        {
                            node.AreChildrenLoaded = true;
                            return loadChildren(node);
                        }
                        else
                        {
                            return reloadChildren(node);
                        }
                    }
                    else if (node.AreChildrenLoaded)
                    {
                        return unloadChildren(node);
                    }
                    return Observable.Empty<INodeViewModel>();
                })
                .Subscribe(child => Globals.UI.Post((a) => node.Add(child), null))
                .DisposeWith(childrenSubscriptions.Get(node.Key(), () => new CompositeDisposable()));

            IObservable<INodeViewModel> unloadChildren(INodeViewModel node)
            {
                if (dictionary.ContainsKey(node) == false)
                {
                    var list = new List<INodeViewModel>();
                    dictionary.Add(node, list);
                    bool b = false;
                    foreach (INodeViewModel child in node.Children)
                    {
                        dictionary[node].Add(child);
                    }
                }
                node.Clear();
                return Observable.Empty<INodeViewModel>();
            }

            IObservable<INodeViewModel> reloadChildren(INodeViewModel node)
            {
                return Observable.Create<INodeViewModel>(observer =>
                {
                    if (dictionary.ContainsKey(node) == false)
                    {
                        throw new Exception("dfg434 cvd44");
                    }
                    else
                    {
                        foreach (INodeViewModel child in dictionary[node])
                        {
                            observer.OnNext(child);
                        }
                    }
                    return Disposable.Empty;
                });
            }

            IObservable<INodeViewModel> loadChildren(INodeViewModel node)
            {
                int childIndex = 0;
                return Observable.Create<INodeViewModel>(observer =>
                {
                    CompositeDisposable disposables = new();

                    if (node is IYieldItems yieldItems)
                    {
                        processYieldItems(yieldItems, observer)
                          .DisposeWith(childrenSubscriptions.Get(node.Key(), () => new CompositeDisposable()));
                    }
                    if (node.IsProliferable)
                    {
                        return repository
                                    .Find((GuidKey)node.Key())
                                    .Subscribe(
                                        change =>
                                        {
                                            if (change.Type == Changes.Type.Add)
                                                processKey(node, change.Value, observer, ref childIndex);
                                            else if (change.Type == Changes.Type.None)
                                            {
                                            }
                                            else throw new Exception("VD");
                                        },
                                        observer.OnError,
                                        () => { observer.OnCompleted(); }
                                        ).DisposeWith(childrenSubscriptions.Get(node.Key(), () => new CompositeDisposable()));
                    }

                    return disposables;
                });

                void processKey(INodeViewModel node, Structs.Repos.Key? key, IObserver<INodeViewModel> observer, ref int childIndex)
                {
                    if (node.IsSingular == false)
                        if (key.HasValue)
                        {
                            var child = nodesStore.Find(key.Value.Guid.ToString());

                            if (child != null)
                            {
                                if (child.IsSingular == false)
                                {
                                    throw new Exception("dddf33s xcc");
                                }
                            }
                            else
                            {
                                findInRepository(node, key.Value.Guid)
                                    .Where(a => a.IsSingular == false)
                                    .Subscribe(observer.OnNext);
                            }
                        }
                        else
                        {
                            //"no child nodes created yet";
                        }
                    else
                    {

                    }
                    //throw new Exception("DSF 33443");
                }

                IDisposable processYieldItems(IYieldItems yieldItems, IObserver<INodeViewModel> observer)
                {
                    CompositeDisposable composite = new();

                    yieldItems
                        .Items()
                        .AndAdditions()
                        .Cast<INodeViewModel>()
                        .Subscribe(child => createChildNode(node, child, ++childIndex, observer).DisposeWith(composite))
                        .DisposeWith(composite);

                    return composite;
                    IDisposable createChildNode(INodeViewModel parent, INodeViewModel child, int index, IObserver<INodeViewModel> observer)
                    {
                        if (child.Name() == null)
                            throw new Exception("child name is null");
                        return repository
                            .Find((GuidKey)parent.Key(), child.Name(), type: GetNodeType(child), index: index)
                            .Subscribe(change =>
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
                                    observer.OnNext(child);
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
            .SelectMany(change =>
            {
                if (change.Type != Changes.Type.Add)
                    throw new Exception("V33d d3D");
                return activateNode(node, change.Value);
            });
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

        private IObservable<INodeViewModel> activateNode(INodeViewModel parent, Structs.Repos.Key? key)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                if (key.HasValue == false)
                    throw new Exception("Key is null");
                keys.Add(key.Value);
                validateKey(key);

                var newNode = (INodeViewModel)dataActivator.Activate(key);
                newNode.SetParent(parent);
                newNode.SetKey(new GuidKey(key.Value.Guid));
                newNode.Removed = key.Value.Removed;

                observer.OnNext(newNode);
                return Disposable.Empty;
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
                .Subscribe(change =>
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
                        if (this.nodesStore.Contains(key.Name))
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
