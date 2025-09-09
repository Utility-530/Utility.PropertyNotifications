using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Core;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Models
{


    public class NodeViewModel : NotifyPropertyClass, INodeViewModel
    {
        private ICollection<IConnectorViewModel> input = new RangeObservableCollection<IConnectorViewModel>();
        private ICollection<IConnectorViewModel> output;

        public event Action<NodeViewModel> InputChanged;

        public event Action? Closed;

        private bool _isVisible;
        private PointF _location;
        private SizeF _size;
        private string? _title;
        private bool _isActive;
        private bool _isSelected;



        public NodeViewModel()
        {
            NewMethod();
            Orientation = Orientation.Horizontal;
            output = new CollectionWithFixedLast<IConnectorViewModel>(new PendingConnectorViewModel() { Node = this });
        }

        public Guid Guid { get; set; }

        public string GroupKey { get; set; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                RaisePropertyChanged(ref _isVisible, value);
                if (!value)
                {
                    Closed?.Invoke();
                }
            }
        }

        public string Key { get; set; }

        public PointF Location
        {
            get => _location;
            set => RaisePropertyChanged(ref _location, value);
        }


        public SizeF Size
        {
            get => _size;
            set => RaisePropertyChanged(ref _size, value);
        }


        public bool IsSelected
        {
            get => _isSelected;
            set => RaisePropertyChanged(ref _isSelected, value);
        }


        public bool IsActive
        {
            get => _isActive;
            set => RaisePropertyChanged(ref _isActive, value);
        }

        public bool IsReadOnly { get; set; }

        public bool IsConnectorsReversed { get; set; }


        private void NewMethod()
        {
            if (input is RangeObservableCollection<IConnectorViewModel> range)
                _ = range.WhenAdded(x =>
                {
                    Add(x);
                });
        }

        void Add(IConnectorViewModel x)
        {
            x.Node = this;
            x.IsInput = true;

        }


        public NodeState State { get; set; } = NodeState.None;

        public object Data { get; set; }

        public ICollection<IConnectorViewModel> Input
        {
            get =>
                input;
            set
            {
                input = value;
                foreach (var inp in value)
                {
                    Add(inp);
                }
                NewMethod();
            }
        }
        public ICollection<IConnectorViewModel> Output
        {
            get => output;
            set
            {
                output = value;
            }
        }

        public IDiagramViewModel Graph { get; set; }
        public Orientation Orientation { get; set; }
    }
}
