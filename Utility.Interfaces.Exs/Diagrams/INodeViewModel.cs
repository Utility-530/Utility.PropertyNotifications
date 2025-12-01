using System.Collections.Generic;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface INodeViewModel : IViewModelTree, IAddCommand, IRemoveCommand
    {
        ICollection<IConnectorViewModel> Inputs { get; }
        ICollection<IConnectorViewModel> Outputs { get; }
        IDiagramViewModel Diagram { get; set; }
    }
}