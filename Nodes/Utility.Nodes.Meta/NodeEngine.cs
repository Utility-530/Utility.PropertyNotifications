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
    public readonly record struct DirtyNode(string Property, INodeViewModel Node);

    public partial class NodeEngine : INodeRoot, IDisposable
    {
        private readonly DataTracker _dataInitialiser;

        private readonly Predicate<INodeViewModel>? childrenTracking;

        // nodes which already came with keys and therefore are not part of this engines repository
        HashSet<Key> keys = new();
        HashSet<Key> roots = new();

        private readonly ITreeRepository repository;
        private readonly INodeSource nodesStore;

        public NodeEngine(ITreeRepository? treeRepo = null, IValueRepository? valueRepository = null, IDataActivator? dataActivator = null, Predicate<INodeViewModel>? childrenTracking = null)
        {
            this.repository = treeRepo ?? Globals.Resolver.Resolve<ITreeRepository>() ?? throw new Exception("££SXXX");
            valueRepository ??=  Globals.Resolver.Resolve<IValueRepository>() ?? throw new Exception("£3__SXXX");
            this.dataActivator = dataActivator ?? Globals.Resolver.Resolve<IDataActivator>() ?? throw new Exception("£3__SXXX");
            var x = Globals.Resolver.Resolve<NodeInterface>();
            nodesStore = Globals.Resolver.Resolve<INodeSource>();
            _dataInitialiser = new(valueRepository, x);
            this.childrenTracking = childrenTracking;
        }
        List<INodeViewModel> inserting = new();
        public IObservable<INodeViewModel> Create(object instance)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                if (instance is string key)
                {
                    return nodesStore[key]
                    .Subscribe(a =>
                    {
                        Create(a).Subscribe(observer);
                    });
                }
                else if (instance is INodeViewModel node)
                {
                    ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));

                    if (node.Guid == default)
                    {
                        throw new Exception("node missing guid!");
                    }

                    var existingNode = nodesStore.Find(node.Key());
                    if (existingNode != null)
                    {
                        observer.OnNext(existingNode);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }
                    if(inserting.Contains(node))
                    {
                        observer.OnNext(node);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }
                    inserting.Add(node);
                    return repository
                           .InsertRoot(node.Guid, node.Name(), GetNodeType(node))
                           .Subscribe(key =>
                           {
                               inserting.Remove(node);
                               if (key.HasValue == false)
                                   throw new Exception("Key is null");
                               keys.Add(key.Value);

                               Add(node);
                               observer.OnNext(node);
                               observer.OnCompleted();
                           });
                }
                throw new Exception("R44sdfds ff");
            });
        }

        public void Destroy(object instance)
        {
            if (instance is string key)
            {
                ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));
                var node = nodesStore.Find(key);

                if (CanRemove(node))
                {
                    nodesStore.Remove(key);
                }
                repository.Remove((GuidKey)key);
            }
        }

        private void Add(INodeViewModel node)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));

            if (shouldIgnoreNode(node)) return;

            if (node.Key() is null && node.Parent()?.Key() != null)
            {
                handleNodeWithoutKey(node);
                return;
            }
            else
            {
            }

            if (nodesStore.Find(node.Key()) != null)
                return;
            nodesStore.Add(node);
            addNodeToCollection(node);

            void handleNodeWithoutKey(INodeViewModel node)
            {
                var index = countSiblingNodesWithSameName(node);
                var findSubscription = repository
                    .Find((GuidKey)node.Parent().Key(), node.Name(), type: GetNodeType(node), index: index == 0 ? null : index)
                    .Subscribe(change =>
                    {
                        if(change.Type == Changes.Type.Add)
                        {
                            validateKey(change.Value);
                            node.SetKey(new GuidKey(change.Value.Guid));
                            Add(node);
                        }
                        else
                            throw new Exception("Node not found in repository");
                    });
            }

            static bool shouldIgnoreNode(INodeViewModel node) => node is IIgnore;

            int countSiblingNodesWithSameName(INodeViewModel node)
            {
                return node.Parent()?.Children.Count(child => ((INodeViewModel)child).Name() == node.Name()) ?? 0;
            }

            //bool nodeAlreadyExists(INodeViewModel node)
            //{
            //    return _nodes.Any(existingNode => existingNode.Key() == node.Key());
            //}
        }

        void addNodeToCollection(INodeViewModel node)
        {
            _dataInitialiser
                .Load(node)
                .Subscribe(_ =>
                {
                    //if (node.IsValueTracked == false)
                    if (childrenTracking?.Invoke(node) != false)
                    {
                        _dataInitialiser.Track(node);
                        if (node.AreChildrenLoaded == false)
                        {
                            setupChildrenTracking(node);
                            setupExpansionHandling(node);
                        }
                    }
                })
                .DisposeWith(_compositeDisposable);

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
                                repository.Remove((GuidKey)change.Value.Key());
                                break;
                        }
                    })
                    .DisposeWith(_compositeDisposable);
            }
        }

        public bool CanRemove(INodeViewModel nodeViewModel)
        {
            return keys.Any(k => k.Guid == ((GuidKey)nodeViewModel.Key()));
        }

        private static void validateKey(Structs.Repos.Key? key)
        {
            if (!key.HasValue)
                throw new Exception(ERROR_KEY_NOT_FOUND);
        }

        private static Type GetNodeType(object node)
        {
            return node is IGetType getType ? getType.GetType() : node.GetType();
        }

        #region Dispose Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _compositeDisposable?.Dispose();
                nodesStore.Dispose();
                //_dirty?.Dispose();
            }

            _disposed = true;
        }

        public IDisposable Subscribe(IObserver<INodeViewModel> observer)
        {
            return Roots().Subscribe(observer);
        }

        #endregion Dispose Pattern
    }
}