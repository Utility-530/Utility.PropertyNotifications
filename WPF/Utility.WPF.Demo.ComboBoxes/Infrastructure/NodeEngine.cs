using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Reactives;
using Utility.PropertyNotifications;
using System.Reactive.Disposables;

namespace Utility.WPF.Demo.ComboBoxes.Infrastructure
{
    class NodeEngine : INodeRoot
    {
        CompositeDisposable _compositeDisposable = new();
        private Dictionary<INodeViewModel, List<INodeViewModel>> dictionary = [];

        public IObservable<INodeViewModel> Create(object model)
        {
            if (model is IEnumerable<INodeViewModel> enumerable)
            {
                return Observable.Create<INodeViewModel>(observer =>
                {
                    foreach (var item in enumerable)
                    {
                        Create(item).Subscribe(observer.OnNext);
                    }
                    return Disposable.Empty;
                });
            }
            if (model is not INodeViewModel node)
                throw new InvalidOperationException("Model is not of type INodeViewModel");
            addNodeToCollection(node);
            return Observable.Return<INodeViewModel>(node);

            void addNodeToCollection(INodeViewModel node)
            {
                if (node is IGetGuid { Guid: Guid guid } && guid == default)
                {
                    (node as ISetGuid).Guid = Guid.NewGuid();
                }
                else 
                {

                }
                if (node.AreChildrenLoaded == false)
                {
                    setupChildrenTracking(node);
                    setupExpansionHandling(node);
                }
            }

            void configureNode(INodeViewModel node)
            {
            }

            void setupChildrenTracking(INodeViewModel node)
            {
                node.Children
                    .AndChanges<INodeViewModel>()
                    .Where(_ => node is not IRoot || node.Count <= 1)
                    .SelectMany(changes => changes)
                    .Subscribe(change =>
                    {
                        switch (change.Type)
                        {
                            case Changes.Type.Add:
                                addNodeToCollection(change.Value);
                                break;
                            case Changes.Type.Remove:
                                //repository.Remove((GuidKey)change.Value.Key());
                                break;
                        }
                    })
                    .DisposeWith(_compositeDisposable);
            }

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
                            yieldItems
                            .Items()
                            .AndAdditions<INodeViewModel>()
                            .Subscribe(a => observer.OnNext(a))
                            .DisposeWith(disposables);
                        }
                        //if (node.IsProliferable)
                        //{
                        //    return repository
                        //                .Find((GuidKey)node.Key())
                        //                .Subscribe(
                        //                    change =>
                        //                    {
                        //                        if (change.Type == Changes.Type.Add)
                        //                            processKey(node, change.Value, observer, ref childIndex);
                        //                        else if (change.Type == Changes.Type.None)
                        //                        {
                        //                        }
                        //                        else throw new Exception("VD");
                        //                    },
                        //                    observer.OnError,
                        //                    () => { observer.OnCompleted(); }
                        //                    ).DisposeWith(disposables);
                        //}

                        return disposables;
                    });

                    //void processKey(INodeViewModel node, Structs.Repos.Key? key, IObserver<INodeViewModel> observer, ref int childIndex)
                    //{
                    //    if (key.HasValue)
                    //    {
                    //        var child = nodesStore.Find(key.Value.Guid.ToString());

                    //        if (child != null)
                    //        {
                    //            if (child.IsSingular == false)
                    //            {
                    //                throw new Exception("dddf33s xcc");
                    //            }
                    //        }
                    //        else
                    //        {
                    //            findInRepository(node, key.Value.Guid).Subscribe(observer.OnNext);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //"no child nodes created yet";
                    //    }
                    //    //throw new Exception("DSF 33443");
                    //}

                    //IDisposable processYieldItems(IYieldItems yieldItems, IObserver<INodeViewModel> observer)
                    //{
                    //    CompositeDisposable composite = new();

                    //    yieldItems
                    //        .Items()
                    //        .AndAdditions< INodeViewModel>()
                    //        .Subscribe(a=>a)
                    //        //.Subscribe(child => createChildNode(node, child, ++childIndex, observer).DisposeWith(composite))
                    //        .DisposeWith(composite);

                    //    return composite;
                    //    //IDisposable createChildNode(INodeViewModel parent, INodeViewModel child, int index, IObserver<INodeViewModel> observer)
                    //    //{
                    //    //    if (child.Name() == null)
                    //    //        throw new Exception("child name is null");
                    //    //    return repository
                    //    //        .Find((GuidKey)parent.Key(), child.Name(), type: GetNodeType(child), index: index)
                    //    //        .Subscribe(change =>
                    //    //        {
                    //    //            if (change.Type != Changes.Type.Add)
                    //    //                throw new Exception("V33D");
                    //    //            //if (change.Value == false)
                    //    //            //    throw new Exception("Key is null");
                    //    //            keys.Add(change.Value);

                    //    //            var existingNode = nodesStore.Find(change.Value.Guid.ToString());
                    //    //            if (existingNode != null)
                    //    //            {
                    //    //                // if current value set then already created
                    //    //            }
                    //    //            else
                    //    //            {
                    //    //                validateKey(change.Value);
                    //    //                child.SetKey(new GuidKey(change.Value.Guid));
                    //    //                observer.OnNext(child);
                    //    //            }
                    //    //        });
                    //    //}
                    //}
                    //}
                }

            }

        }

        public void Destroy(object key)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<INodeViewModel> observer)
        {
            throw new NotImplementedException();
        }
    }
}
