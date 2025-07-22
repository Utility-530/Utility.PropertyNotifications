using System.Drawing;
using System.Windows;
using Utility.Models;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Models
{
    public class PendingConnectionViewModel : NotifyPropertyClass
    {
        private ConnectorViewModel _source = default!, _target;
        private bool _isVisible;
        private PointF _targetLocation;

        public ConnectorViewModel Source
        {
            get => _source;
            set => RaisePropertyChanged(ref _source, value);
        }

        public ConnectorViewModel? Target
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
