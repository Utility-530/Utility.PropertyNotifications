using System;
using System.Windows;
using Utility.ViewModels.Base;

namespace Utility.Nodify.Core
{
    public class ConnectorViewModel : BaseViewModel, IConnectorViewModel
    {
        private INodeViewModel _node = default!;

        private string? _title;
        private object _value;
        private bool _isConnected;
        private bool _isInput;
        private Point _anchor;
        private Type type;


        public string? Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public object Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => Set(ref _isConnected, value);
        }

        public Type Type
        {
            get => type;
            set => Set(ref type, value);
        }

        public bool IsInput
        {
            get => _isInput;
            set => Set(ref _isInput, value);
        }

        public Point Anchor
        {
            get => _anchor;
            set => Set(ref _anchor, value);
        }


        public INodeViewModel Node
        {
            get => _node;
            set => Set(ref _node, value);
        }

    }
}
