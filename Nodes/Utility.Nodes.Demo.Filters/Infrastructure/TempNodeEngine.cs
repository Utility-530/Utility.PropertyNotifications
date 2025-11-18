using System.Reactive.Linq;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Structs.Repos;
using Key = Utility.Structs.Repos.Key;

namespace Utility.Nodes.Demo.Filters.Infrastructure
{
    internal class TempNodeEngine : INodeSource
    {
        private static HashSet<INodeViewModel> nodes = new();

        public TempNodeEngine()
        {
        }

        public string New { get; }

        public void Add(INodeViewModel node)
        {

            //_node.Key = new GuidKey();
            if (nodes.Add(node) == false)
                return;

            if(node is IGetKey { Key: null })
            {
                if (node is ISetKey setKey)
                {
                    setKey.Key = new GuidKey(); 
                }
            }   

            node.Children.AndChanges<INodeViewModel>().Subscribe(set =>
            {
                foreach (var change in set)
                {
                    if (change.Type == Changes.Type.Add)
                    {
                        Add(change.Value);
                    }
                }
            });
            if (node is IYieldItems children)
            {
                node.WithChangesTo(a => a.IsExpanded)
                    .Where(a => a)
                    .Take(1)
                    .Subscribe(a =>
                    {
                        children
                            .Items()
                            .AndAdditions()
                            .Subscribe(async d =>
                            {
                                if (node.Any(a => a.ToString().Equals(d.ToString())))
                                {
                                    return;
                                }
                                var _new = (INodeViewModel)await node.ToTree(d);
                                node.Add(_new);
                            });
                    });
            }


        }

        public IObservable<INodeViewModel> Create(string name, Guid guid, Func<string, object> modelFactory)
        {
            throw new NotImplementedException();
        }

        public IObservable<Key?> Find(Guid parentGuid, string name, Guid? guid = null, Type? type = null, int? localIndex = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<DateValue> Get(Guid guid, string name)
        {
            throw new NotImplementedException();
        }

        public int? MaxIndex(Guid guid, string v)
        {
            throw new NotImplementedException();
        }

        public DateTime Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Remove(INodeViewModel node)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public IObservable<INodeViewModel?> Single(string v)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IObservable<INodeViewModel> FindChild(INodeViewModel node, Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Remove(Predicate<INodeViewModel> predicate)
        {
            throw new NotImplementedException();
        }

        public IObservable<INodeViewModel> Roots()
        {
            throw new NotImplementedException();
        }

        public bool CanRemove(INodeViewModel nodeViewModel)
        {
            throw new NotImplementedException();
        }

        public static TempNodeEngine Instance { get; } = new TempNodeEngine();
        public IReadOnlyCollection<INodeViewModel> Nodes { get; }
        public IObservable<INodeViewModel> Selections { get; }
    }
}
