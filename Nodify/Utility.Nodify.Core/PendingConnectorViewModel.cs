using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Input;
using Utility.Commands;
using Utility.Nodify.Core;
using Utility.Nodify.Enums;

namespace Utility.Nodify.Models
{
    public class PendingConnectorViewModel : IConnectorViewModel
    {
        public PendingConnectorViewModel()
        {
            AddConnectorCommand = new Command<object>((a) =>
            {
                if (a is PropertyInfo propertyInfo)
                    this.Node.Output.Add(new ConnectorViewModel() { Data = propertyInfo, Key = propertyInfo.Name, Node = this.Node });
            });
        }
        public bool IsConnected { get; set; }
        public object Data { get; set; }
        public bool IsInput { get; set; }
        public INodeViewModel Node { get; set; }
        public IReadOnlyCollection<IConnectionViewModel> Connections { get; }
        public PointF Anchor { get; set; }
        public ConnectorFlow Flow { get; }
        public ConnectorShape Shape { get; }
        public string Key { get; }
        public Guid Guid { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand AddConnectorCommand { get; }
    }
}
