using System;
using System.Collections.Generic;
using Utility.Structs.Repos;
using Utility.Trees.Abstractions;

namespace Utility.Interfaces.Exs
{
    public interface INodeSource: IDisposable
    {
        string New { get; }
        IReadOnlyCollection<INode> Nodes { get; }

        void Remove(INode node);

        void Add(INode node);
        IObservable<INode?> Single(string v);

        void Save();
        IObservable<INode> Create(string name, Guid guid, Func<string, object> modelFactory);
        IObservable<INode> FindChild(INode node, Guid guid);
        IObservable<INode> Selections { get; }
    }
}
