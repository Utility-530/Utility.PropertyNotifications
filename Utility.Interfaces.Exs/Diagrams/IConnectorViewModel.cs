using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;

namespace Utility.Interfaces.Exs.Diagrams
{
    public interface IValueConnectorViewModel : IConnectorViewModel, IValue
    {
    }

    public interface IConnectorViewModel : INodeViewModel
    {
        bool IsConnected { get; set; }
        bool IsInput { get; set; }
        INodeViewModel Node { get; set; }
        PointF Anchor { get; set; }
        IO Flow { get; }
        FlatShape Shape { get; }
    }
}