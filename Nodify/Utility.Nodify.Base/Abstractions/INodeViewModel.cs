using System;
using System.Drawing;
using System.Windows;
using Utility.Enums;

namespace Utility.Nodify.Core
{
    public interface INodeViewModel
    {
        Guid Id { get; }
        ICollection<IConnectorViewModel> Input { get; }
        ICollection<IConnectorViewModel> Output { get; }
        string? Title { get; set; }
        PointF Location { get; set; }
        NodeState State { get; set; }
        Key Key { get; }
        ICore Core { get; set; }
        IDiagramViewModel Graph { get; set; }
        Orientation Orientation { get; }
    }
}