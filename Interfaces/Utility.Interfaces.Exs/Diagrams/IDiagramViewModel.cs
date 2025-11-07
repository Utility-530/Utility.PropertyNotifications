using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface IDiagramViewModel : IKey, IGuid
    {
        ObservableCollection<IConnectionViewModel> Connections { get; }
        ObservableCollection<INodeViewModel> Nodes { get; }
    }
}