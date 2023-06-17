using System.Collections.Generic;
using System.Windows;
using Utility.Models;

namespace Utility.Nodify.Core
{
    public class ConnectorViewModel : BaseViewModel
    {
        private NodeViewModel _node = default!;

        private string? _title;
        private object _value;
        private bool _isConnected;
        private bool _isInput;
        private Point _anchor;


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


        public NodeViewModel Node
        {
            get => _node;
            set => Set(ref _node, value);
        }

    }
}
