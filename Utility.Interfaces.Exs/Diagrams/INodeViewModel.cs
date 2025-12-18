using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface INodeViewModel : IViewModelTree, IAddCommand, IRemoveCommand
    {
        ICollection<INodeViewModel> Nodes { get; }
        ICollection<IConnectionViewModel> Connections { get; }
        ICollection<IConnectorViewModel> Inputs { get; }
        ICollection<IConnectorViewModel> Outputs { get; }
        INodeViewModel Diagram { get; set; }
    }
}