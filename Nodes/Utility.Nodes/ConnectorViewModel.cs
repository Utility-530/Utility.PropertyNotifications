using System.Drawing;
using System.Reflection;
using Utility.Collections;
using Utility.Enums;
using Utility.Helpers.Generic;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class ConnectorViewModel : NodeViewModel, IValueConnectorViewModel, IValue, IType
    {
        private ThreadSafeObservableCollection<IConnectionViewModel> connections = [];
        private INodeViewModel _node = default!;

        private bool _isConnected;
        private bool _isInput;
        private PointF _anchor;

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
            IsExpanded = false;
        }

        public override Type Type => Data is Type type ? type : Data is IParameterInfo parameterInfo ?
            parameterInfo?.Parameter?.ParameterType ?? typeof(string):
            Data is PropertyInfo propertyInfo ? propertyInfo?.PropertyType :
            typeof(string);//throw new Exception("ds322d 11");

        public bool IsConnected
        {
            get => _isConnected;
            set => RaisePropertyChanged(ref _isConnected, value);
        }

        public bool IsInput
        {
            get => _isInput;
            set => RaisePropertyChanged(ref _isInput, value);
        }

        public PointF Anchor
        {
            get => _anchor;
            set => RaisePropertyChanged(ref _anchor, new PointF(value.X, value.Y));
        }

        public INodeViewModel Node
        {
            get => _node;
            set => RaisePropertyChanged(ref _node, value).Then(a => OnNodeChanged());
        }

        public FlatShape Shape { get; set; }

        public IO Flow { get; set; }

        public int MaxConnections { get; set; } = 2;

        public override ICollection<IConnectionViewModel> Connections => connections;

        public object AnchorElement { get; set; }

        protected virtual void OnNodeChanged()
        {
            if (Node is INodeViewModel flow)
            {
                Flow = flow.Inputs.Contains(this) ? IO.Input : IO.Output;
            }
        }

        public bool IsConnectedTo(ConnectorViewModel con)
            => Connections.Any(c => c.Input == con || c.Output == con);

        public virtual bool AllowsNewConnections()
            => Connections.Count < MaxConnections;

        public void Disconnect()
        { }
    }
}