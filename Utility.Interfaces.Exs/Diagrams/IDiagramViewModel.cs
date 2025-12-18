using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface IDiagramViewModel : INodeViewModel
    {
         ICollection<INodeViewModel> SelectedNodes { get; }

         IConnectionViewModel PendingConnection { get; set; }
    }
}