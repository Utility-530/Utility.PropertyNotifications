using System;
using System.Collections.Generic;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Interfaces.Exs
{
    public interface INodeSource : IDisposable
    {
        string New { get; }

        IReadOnlyCollection<INodeViewModel> Nodes { get; }

        void Add(INodeViewModel node);

        void Remove(Predicate<INodeViewModel> predicate);

        IObservable<INodeViewModel?> Single(string v);

        IObservable<INodeViewModel> Create(string name, Guid guid, Func<string, object> modelFactory);

        IObservable<INodeViewModel> FindChild(INodeViewModel node, Guid guid);

        IObservable<INodeViewModel> Roots();

        bool CanRemove(INodeViewModel nodeViewModel);

        IObservable<INodeViewModel> Selections { get; }
    }
}