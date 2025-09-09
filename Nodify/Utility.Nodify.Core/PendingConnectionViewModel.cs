using System;
using System.Drawing;
using System.Windows;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodify.Core;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Models
{
    public class PendingConnectionViewModel : NotifyPropertyClass, IConnectionViewModel
    {
        private IConnectorViewModel _source = default!, _target;
        private bool _isVisible;
        private PointF _targetLocation;

        public string Key { get; set; }

        public Guid Guid { get; set; }

        public IConnectorViewModel Input
        {
            get => _source;
            set => RaisePropertyChanged(ref _source, value);
        }

        public IConnectorViewModel? Output
        {
            get => _target;
            set => RaisePropertyChanged(ref _target, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => RaisePropertyChanged(ref _isVisible, value);
        }

        public PointF TargetLocation
        {
            get => _targetLocation;
            set => RaisePropertyChanged(ref _targetLocation, value);
        }
    }
}
