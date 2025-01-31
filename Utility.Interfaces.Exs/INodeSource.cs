using System;
using Utility.Interfaces.Exs;
using Utility.Structs.Repos;

namespace Utility.Nodes.Filters
{
    public interface INodeSource
    {
        string New { get; }

        DateTime Remove(Guid guid);
        IObservable<INode> ChildrenByGuidAsync(Guid guid);
        int? MaxIndex(Guid guid, string v);
        void Remove(INode node);
        IObservable<Key?> Find(Guid parentGuid, string name, Guid? guid = null, System.Type? type = null, int? localIndex = null);
        IObservable<DateValue> Get(Guid guid, string name);
        void Set(Guid guid, string name, object value, DateTime dateTime );
        void Add(INode node);
        IObservable<INode?> Single(string v);
    }
}
