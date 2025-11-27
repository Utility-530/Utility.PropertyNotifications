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
using CompositeDisposable = Utility.Observables.CompositeDisposable;
using Key = Utility.Structs.Repos.Key;

namespace Utility.Nodes.Meta
{
    public partial class NodeEngine
    {
        private const string ERROR_KEY_NOT_FOUND = "Key not found";
        private readonly Utility.Observables.CompositeDisposable _compositeDisposable = new();
        private readonly Dictionary<INodeViewModel, List<INodeViewModel>> dictionary = new();

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
         .DisposeWith(_compositeDisposable);

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
                            .DisposeWith(disposables);
                    }
                    if (node.IsProliferable)
                    {
                        return repository
                                    .Find((GuidKey)node.Key())
                                    .Subscribe(
                                        key => processKey(node, key, observer, ref childIndex),
                                        observer.OnError,
                                        () => { observer.OnCompleted(); }
                                        ).DisposeWith(disposables);
                    }

                    return disposables;
                });

                void processKey(INodeViewModel node, Structs.Repos.Key? key, IObserver<INodeViewModel> observer, ref int childIndex)
                {
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
                            findInRepository(node, key.Value.Guid).Subscribe(observer.OnNext);
                        }
                    }
                    else
                    {
                        //"no child nodes created yet";
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
                            .Subscribe(key =>
                            {
                                if (key.HasValue == false)
                                    throw new Exception("Key is null");
                                keys.Add(key.Value);

                                var existingNode = nodesStore.Find(key.Value.Guid.ToString());
                                if (existingNode != null)
                                {
                                    // if current value set then already created
                                }
                                else
                                {
                                    validateKey(key);
                                    child.SetKey(new GuidKey(key.Value.Guid));
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
            .SelectMany(key => activateNode(node, key));
        }


        public IObservable<INodeViewModel> FindChild(INodeViewModel node, Guid guid)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                var child = nodesStore.Find(guid.ToString());
                {
                    if (child != null)
                    {
                        Add(child);
                        return Disposable.Empty;
                    }
                    else
                    {
                        // Search in repository
                        return findInRepository(node, guid)
                             .Subscribe(activatedNode =>
                             {
                                 observer.OnNext(activatedNode);
                                 observer.OnCompleted();
                             });
                    }
                    //return new CompositeDisposable(repositorySubscription);
                }
                //_nodes.AndAdditions().Subscribe(addition =>
                //{
                //    if (addition.Guid == guid)
                //    {
                //        observer.OnNext(addition);
                //        observer.OnCompleted();
                //    }
                //}).DisposeWith(_compositeDisposable);
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

                var newNode = (INodeViewModel)DataActivator.Activate(key);
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
                return repository
                       .SelectKeys()
                       .Subscribe(keys =>
                       {
                           foreach (var key in keys)
                           {
                               if (key == default)
                                   throw new Exception("Key is null");
                               this.keys.Add(key);
                               this.roots.Add(key);
                               var existingNode = nodesStore.Find(key.Guid.ToString());
                               if (existingNode != null)
                               {
                                   // if current value set then already created
                               }
                               else if (key.Removed.HasValue)
                               {

                               }
                               else
                               {
                                   var child = (INodeViewModel)DataActivator.Activate(key);
                                   validateKey(key);
                                   child.SetKey(new GuidKey(key.Guid));
                                   child.IsProliferable = true;
                                   Add(child);
                                   observer.OnNext(child);
                               }
                           }
                       }, () => observer.OnCompleted());
            });
        }
    }
}
