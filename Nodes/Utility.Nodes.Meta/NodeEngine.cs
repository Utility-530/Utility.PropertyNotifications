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

    public class NodeEngine : INodeSource, IDisposable
    {
        private const string ERROR_KEY_NOT_FOUND = "Key not found";
        private const string ERROR_INVALID_GUID = "Invalid GUID format";

        public static string New => "new";
        public static readonly string Key = nameof(NodeEngine);
        public static NodeEngine Instance { get; } = new();

        private readonly ReplaySubject<INodeViewModel> _selections = new(1);
        //private readonly ReplaySubject<DirtyModel> _dirty = new(1);
        private readonly ObservableCollection<INodeViewModel> _nodes = [];
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly ChangeTracker _changeTracker;
        private readonly DataTracker _dataInitialiser;


        private readonly Lazy<ITreeRepository> _repository;
        private readonly Predicate<INodeViewModel>? childrenTracking;

        // nodes which already came with keys and therefore are not part of this engines repository
        HashSet<Key> keys = new();
        HashSet<Key> roots = new();

        private bool _disposed;

        public NodeEngine(ITreeRepository? treeRepo = null, Predicate<INodeViewModel>? childrenTracking = null)
        {
            _changeTracker = new(this);

            var repo = treeRepo ?? Globals.Resolver.Resolve<ITreeRepository>() ?? throw new Exception("££SXXX");
            _repository = new(() => repo);
            _dataInitialiser = new(repo, new NodeInterface(this));
            this.childrenTracking = childrenTracking;
        }

        public Guid Guid { get; } = Guid.NewGuid();
        public IReadOnlyCollection<INodeViewModel> Nodes => _nodes;
        string INodeSource.New => New;
        public IObservable<INodeViewModel> Selections => _selections;

        public void Remove(Predicate<INodeViewModel> predicate)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));
            var node = _nodes.SingleOrDefault(a => predicate(a));

            if (CanRemove(node))
            {
                _nodes.Remove(node);
                _repository.Value.Remove((GuidKey)node.Key());
            }
        }

        public void Add(INodeViewModel node)
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

            if (nodeAlreadyExists(node)) return;

            addNodeToCollection(node);

            void handleNodeWithoutKey(INodeViewModel node)
            {
                var index = countSiblingNodesWithSameName(node);
                var findSubscription = _repository.Value
                    .Find((GuidKey)node.Parent().Key(), node.Name(), type: GetNodeType(node), index: index == 0 ? null : index)
                    .Subscribe(key =>
                    {
                        validateKey(key);
                        node.SetKey(new GuidKey(key.Value.Guid));
                        Add(node);
                    });
            }

            static bool shouldIgnoreNode(INodeViewModel node) => node is IIgnore;

            int countSiblingNodesWithSameName(INodeViewModel node)
            {
                return node.Parent()?.Children.Count(child => ((INodeViewModel)child).Name() == node.Name()) ?? 0;
            }
            bool nodeAlreadyExists(INodeViewModel node)
            {
                return _nodes.Any(existingNode => existingNode.Key() == node.Key());
            }

            void addNodeToCollection(INodeViewModel node)
            {
                _nodes.Add(node);

                _dataInitialiser
                    .Load(node)
                    .Subscribe(_ =>
                    {
                        //if (node.IsValueTracked == false)
                        _dataInitialiser.Track(node);

                        if (node.AreChildrenLoaded == false)
                            if (childrenTracking == null || childrenTracking(node))
                            {
                                setupChildrenTracking(node);
                                setupExpansionHandling(node);                     
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
                        .Subscribe(_changeTracker.Track)
                        .DisposeWith(_compositeDisposable);
                }

                void setupExpansionHandling(INodeViewModel node)
                {
                    node.WhenReceivedFrom(n => n.IsExpanded)
                        .Where(isExpanded => isExpanded)
                        .Take(1)
                        .SelectMany(_ => loadChildren(node))
                        .Subscribe(child => node.Add(child), () => node.AreChildrenLoaded = true)
                        .DisposeWith(_compositeDisposable);

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
                                return _repository.Value
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
                                findChild(node, key.Value.Guid)
                                    .Subscribe(child =>
                                    {
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
                                    });
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
                                return _repository.Value
                                    .Find((GuidKey)parent.Key(), child.Name(), type: GetNodeType(child), index: index)
                                    .Subscribe(key =>
                                    {
                                        if (key.HasValue == false)
                                            throw new Exception("Key is null");
                                        keys.Add(key.Value);

                                        var existingNode = _nodes.SingleOrDefault(a => a.Key() == key.Value.Guid.ToString());
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
            }
        }

        public bool CanRemove(INodeViewModel nodeViewModel)
        {
            return keys.Any(k => k.Guid == ((GuidKey)nodeViewModel.Key()));
        }

        private IObservable<INodeViewModel> findInRepository(INodeViewModel node, Guid guid)
        {
            return _repository.Value
            .Find((GuidKey)node.Key(), guid: guid)
            .SelectMany(key => activateNode(node, key));
        }

        public IObservable<INodeViewModel> FindChild(INodeViewModel node, Guid guid)
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                var _compositeDisposable = new CompositeDisposable();
                findChild(node, guid).Subscribe(child =>
                {
                    if (child != null)
                        Add(child);
                    else
                    {
                        // Search in repository
                        var repositorySubscription = findInRepository(node, guid)
                            .Subscribe(activatedNode =>
                            {
                                observer.OnNext(activatedNode);
                                observer.OnCompleted();
                            });
                    }
                    //return new CompositeDisposable(repositorySubscription);
                }).DisposeWith(_compositeDisposable);
                //_nodes.AndAdditions().Subscribe(addition =>
                //{
                //    if (addition.Guid == guid)
                //    {
                //        observer.OnNext(addition);
                //        observer.OnCompleted();
                //    }
                //}).DisposeWith(_compositeDisposable);
                return _compositeDisposable;
            });
        }

        private IObservable<INodeViewModel> findChild(INodeViewModel node, Guid guid)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));

            return Observable.Create<INodeViewModel>(observer =>
            {
                // Check if node already exists in collection
                var existingNode = _nodes.SingleOrDefault(a => a.Key() == guid.ToString());
                if (existingNode != null)
                {
                    observer.OnNext(existingNode);
                    //throw new Exception("FF 333ccd");
                    observer.OnCompleted();
                    return Disposable.Empty;
                }
                observer.OnNext(null);
                observer.OnCompleted();
                return Disposable.Empty;
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

        public IObservable<INodeViewModel> Single(string key) => Many(key).Take(1);

        public IObservable<INodeViewModel> Many(string key)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));

            return Observable.Create<INodeViewModel>(observer =>
            {
                if (!Guid.TryParse(key, out var _))
                    throw new Exception(ERROR_INVALID_GUID);

                return _nodes
                    .AndAdditions<INodeViewModel>()
                    .Where(node => node.Key().Equals(key))
                    .Take(1)
                    .Subscribe(node =>
                    {
                        observer.OnNext(node);
                        observer.OnCompleted();
                    });
            });
        }

        public IObservable<INodeViewModel> Create(string name, Guid guid, Func<string, object> modelFactory)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(NodeEngine));

            return Observable.Create<INodeViewModel>(observer =>
            {
                var existingNode = _nodes.SingleOrDefault(a => (GuidKey)a.Key() == guid);
                if (existingNode != null)
                {
                    observer.OnNext(existingNode);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                var node = createNewNode(name, guid, modelFactory);


                return _repository
                       .Value
                       .InsertRoot(guid, name, GetNodeType(node))
                       .Subscribe(key =>
                        {
                            if (key.HasValue == false)
                                throw new Exception("Key is null");
                            keys.Add(key.Value);

                            Add(node);
                            observer.OnNext(node);
                            observer.OnCompleted();
                        });
            });

            static INodeViewModel createNewNode(string name, Guid guid, Func<string, object> modelFactory)
            {
                var data = (ISetKey)modelFactory(name);
                data.Key = new GuidKey(guid);
                return (INodeViewModel)data;
            }
        }

        public IObservable<INodeViewModel> Roots()
        {
            return Observable.Create<INodeViewModel>(observer =>
            {
                return _repository
                       .Value
                       .SelectKeys()
                       .Subscribe(keys =>
                       {
                           foreach (var key in keys)
                           {
                               if (key == default)
                                   throw new Exception("Key is null");
                               this.keys.Add(key);
                               this.roots.Add(key);
                               var existingNode = _nodes.SingleOrDefault(a => a.Key() == key.Guid.ToString());
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

        #region Private Methods

        private static void validateKey(Structs.Repos.Key? key)
        {
            if (!key.HasValue)
                throw new Exception(ERROR_KEY_NOT_FOUND);
        }

        private static Type GetNodeType(object node)
        {
            return node is IGetType getType ? getType.GetType() : node.GetType();
        }

        #endregion Private Methods

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
                _nodes?.Clear();
                _selections?.Dispose();
                //_dirty?.Dispose();
            }

            _disposed = true;
        }

        #endregion Dispose Pattern
    }

    public class MethodValue
    {
        public bool IsAccessed => Task != null || Nodes != null;
        public object Instance { get; set; }
        public Method Method { get; set; }
        public Task Task { get; set; }
        public IList<INodeViewModel> Nodes { get; set; }
    }
}