using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using Utility.Collections;
using Utility.Helpers.Generic;
using Utility.Nodify.Core;
using Utility.Nodify.Enums;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Models
{
    public class ConnectorViewModel : NotifyPropertyClass, IConnectorViewModel
    {
        ThreadSafeObservableCollection<IConnectionViewModel> connections = [];
        private INodeViewModel _node = default!;

        private string? _title;
        private object data;
        private bool _isConnected;
        private bool _isInput;
        private PointF _anchor;
        private Type type;


        public string? Title
        {
            get => _title;
            set => RaisePropertyChanged(ref _title, value);
        }

        public object Data
        {
            get => data;
            set => RaisePropertyChanged(ref data, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => RaisePropertyChanged(ref _isConnected, value);
        }

        public Type Type
        {
            get => type;
            set => RaisePropertyChanged(ref type, value);
        }

        public bool IsInput
        {
            get => _isInput;
            set => RaisePropertyChanged(ref _isInput, value);
        }

        public PointF Anchor
        {
            get => _anchor;
            set => RaisePropertyChanged(ref _anchor, new PointF(value.X , value.Y));
        }


        public INodeViewModel Node
        {
            get => _node;
            set => RaisePropertyChanged(ref _node, value).Then(a => OnNodeChanged());
        }
        public ConnectorShape Shape { get; set; }

        public ConnectorFlow Flow { get; set; }

        public int MaxConnections { get; set; } = 2;

        public IReadOnlyCollection<IConnectionViewModel> Connections => connections;

        public object AnchorElement { get; set; }

        public ConnectorViewModel()
        {
            connections.WhenAdded(c =>
            {
                c.Input.IsConnected = true;
                c.Output.IsConnected = true;
            }).WhenRemoved(c =>
            {
                if (c.Input.Connections.Count == 0)
                {
                    c.Input.IsConnected = false;
                }

                if (c.Output.Connections.Count == 0)
                {
                    c.Output.IsConnected = false;
                }
            });
        }

        protected virtual void OnNodeChanged()
        {
            if (Node is NodeViewModel flow)
            {
                Flow = flow.Input.Contains(this) ? ConnectorFlow.Input : ConnectorFlow.Output;
            }

            //else if (Node is KnotNodeViewModel knot)
            //{
            //    Flow = knot.Flow;
            //}
        }

        public bool IsConnectedTo(ConnectorViewModel con)
            => Connections.Any(c => c.Input == con || c.Output == con);

        public virtual bool AllowsNewConnections()
            => Connections.Count < MaxConnections;

        public void Disconnect()
        { }
        //=> Node.Graph.Schema.DisconnectConnector(this);
    }
}
