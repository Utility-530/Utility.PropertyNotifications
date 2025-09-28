using System;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodes.Meta
{
    internal interface IDataInitialiser
    {
        IObservable<INodeViewModel> Load(INodeViewModel node);
        void Track(INodeViewModel node);
    }
}