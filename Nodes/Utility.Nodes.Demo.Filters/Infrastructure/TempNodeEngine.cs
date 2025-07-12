using System.Reactive.Linq;
using Utility.Interfaces.Exs;
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
        private static HashSet<INode> nodes = new();

        public TempNodeEngine()
        {
        }

        public string New { get; }

        public void Add(INode node)
        {

            //_node.Key = new GuidKey();
            if (nodes.Add(node) == false)
                return;

            node.Key ??= new GuidKey();

            if (node is INode { Data: ISetNode setNode })
            {
                setNode.SetNode(node);

            }

            node.Items.AndChanges<INode>().Subscribe(set =>
            {
                foreach (var change in set)
                {
                    if (change.Type == Changes.Type.Add)
                    {
                        Add(change.Value);
                    }
                }
            });
            if (node.Data is IYieldChildren children)
            {
                node.WithChangesTo(a => a.IsExpanded)
                    .Where(a => a)
                    .Take(1)
                    .Subscribe(a =>
                    {
                        children
                            .Children
                            .AndAdditions()
                            .Subscribe(async d =>
                            {
                                if (node.Any(a => a.Data.ToString().Equals(d.ToString())))
                                {
                                    return;
                                }
                                var _new = (INode)await node.ToTree(d);
                                node.Add(_new);
                            });
                    });
            }


        }

        public IObservable<INode> Create(string name, Guid guid, Func<string, INode> nodeFactory, Func<string, object> modelFactory)
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

        public void Remove(INode node)
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

        public IObservable<INode?> Single(string v)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IObservable<INode> FindChild(INode node, Guid guid)
        {
            throw new NotImplementedException();
        }

        public static TempNodeEngine Instance { get; } = new TempNodeEngine();
        public IReadOnlyCollection<INode> Nodes { get; }
        public IObservable<INode> Selections { get; }
    }
}
