using System;
using Utility.Interfaces.Exs;
using Utility.Structs.Repos;

namespace Utility.Nodes.Filters
{
    public interface INodeSource
    {
        string New { get; }

        void Remove(Guid guid);
        IObservable<INode> ChildrenByGuidAsync(Guid guid);
        IObservable<INode> SingleByGuidAsync(object currentGuid);
        int? MaxIndex(Guid guid, string v);
        void Remove(INode node);
        IObservable<Key> Find(Guid guid, string name, System.Type type, object localIndex);
        IObservable<GuidValue> Get(Guid guid);
        void Add(INode node);
    }
}
