using System;
using System.Drawing;
using System.Windows;
using Utility.Enums;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodify.Core
{
    public interface INodeViewModel: IKey, IId<Guid>, IData
    {
        ICollection<IConnectorViewModel> Input { get; }
        ICollection<IConnectorViewModel> Output { get; }
        PointF Location { get; set; }
        NodeState State { get; set; }
        IDiagramViewModel Graph { get; set; }
        Orientation Orientation { get; }
    }
}