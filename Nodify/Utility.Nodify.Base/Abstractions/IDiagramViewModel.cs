using System.Collections.ObjectModel;

namespace Utility.Nodify.Core
{
    public interface IDiagramViewModel
    {
        ICollection<IConnectionViewModel> Connections { get; }
        ICollection<INodeViewModel> Nodes { get;  }
    }
}