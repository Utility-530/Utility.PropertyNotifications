using System;
using System.ComponentModel;
using System.Drawing;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Enums;

namespace Utility.Nodify.Core
{
    public interface IConnectorViewModel : IGetKey, INotifyPropertyChanged, IGuid
    {
        bool IsConnected { get; set; }
        object Data { get; set; }
        bool IsInput { get; set; }
        INodeViewModel Node { get; set; }        
        IReadOnlyCollection<IConnectionViewModel> Connections { get; }
        PointF Anchor { get; set; }
        ConnectorFlow Flow { get; }
        ConnectorShape Shape { get; }
    }
}