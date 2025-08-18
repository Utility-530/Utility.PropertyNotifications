using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodify.Core
{
    public interface IDiagramViewModel: IKey
    {
        ICollection<IConnectionViewModel> Connections { get; }
        ICollection<INodeViewModel> Nodes { get;  }
    }
}