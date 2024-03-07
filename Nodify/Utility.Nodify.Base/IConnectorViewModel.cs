using System;
using System.ComponentModel;

namespace Utility.Nodify.Core
{
    public interface IConnectorViewModel : INotifyPropertyChanged
    {
        bool IsConnected { get; set; }
        string? Title { get; set; }
        Type Type { get; set; }
        object Value { get; set; }
        bool IsInput { get; set; }
        INodeViewModel Node { get; set; }
    }
}