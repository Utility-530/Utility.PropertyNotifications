using System;
using System.Collections.Generic;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Structs.Repos;
using Utility.Trees.Abstractions;

namespace Utility.Interfaces.Exs
{
    public interface INodeSource: IDisposable
    {
        string New { get; }
        IReadOnlyCollection<INodeViewModel> Nodes { get; }

        void Remove(INodeViewModel node);

        void Add(INodeViewModel node);
        IObservable<INodeViewModel?> Single(string v);

        void Save();
        IObservable<INodeViewModel> Create(string name, Guid guid, Func<string, object> modelFactory);
        IObservable<INodeViewModel> FindChild(INodeViewModel node, Guid guid);
        void RemoveBy(Predicate<INodeViewModel> predicate);

        IObservable<INodeViewModel> Selections { get; }
    }
}
