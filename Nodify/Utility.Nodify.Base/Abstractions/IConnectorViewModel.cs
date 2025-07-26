using System;
using System.ComponentModel;
using Utility.Nodify.Enums;

namespace Utility.Nodify.Core
{
    public interface IConnectorViewModel : INotifyPropertyChanged
    {
        bool IsConnected { get; set; }
        string? Title { get; set; }
        Type Type { get; set; }
        object Data { get; set; }
        bool IsInput { get; set; }
        INodeViewModel Node { get; set; }
        IReadOnlyCollection<IConnectionViewModel> Connections { get; }
        ConnectorFlow Flow { get; }
        ConnectorShape Shape { get; }
    }
}