using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Interfaces.Exs
{

    public interface INodeRoot : IObservable<INodeViewModel>
    {
        IObservable<INodeViewModel> Create(object model);
        void Destroy(object key);
    }

    public interface INodeSource : IDisposable
    {
        IReadOnlyCollection<INodeViewModel> Nodes { get; }
    }
    public static class NodeStoreHelper
    {
        public static INodeViewModel? Find(this INodeSource nodeStore, string key)
        {
            return nodeStore.Nodes.SingleOrDefault(a => a.Key() == key);
        }
    }

}