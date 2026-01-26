using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface INodeViewModel : IViewModelTree, IGuid, IName, IAddCommand, IRemoveCommand
    {
        ICollection<INodeViewModel> Nodes { get; }
        ICollection<IConnectionViewModel> Connections { get; }
        ICollection<IConnectorViewModel> Inputs { get; }
        ICollection<IConnectorViewModel> Outputs { get; }
        INodeViewModel Diagram { get; set; }

        void OpenAt(PointF targetLocation);

        void Close();
    }
}