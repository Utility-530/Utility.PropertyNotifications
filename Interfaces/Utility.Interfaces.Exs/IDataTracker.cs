using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Interfaces.Exs
{
    public interface IDataTracker
    {
        IObservable<INodeViewModel> Load(INodeViewModel node);
        void Track(INodeViewModel node);
    }
}
