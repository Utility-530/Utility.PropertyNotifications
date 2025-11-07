using System.Collections.Generic;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface INodeViewModel : IViewModelTree, IAddCommand, IRemoveCommand
    {
        ICollection<IConnectorViewModel> Input { get; }
        ICollection<IConnectorViewModel> Output { get; }
        IDiagramViewModel Diagram { get; set; }
    }
}