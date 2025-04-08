using System;
using Utility.Structs.Repos;

namespace Utility.Interfaces.Exs
{
    public interface INodeSource: IDisposable
    {
        string New { get; }
        
        void Remove(INode node);

        void Add(INode node);
        IObservable<INode?> Single(string v);

        void Save();
        IObservable<INode> Create(string name, Guid guid, Func<string, INode> nodeFactory, Func<string, object> modelFactory);
    }
}
