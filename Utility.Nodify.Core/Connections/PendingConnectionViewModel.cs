using System.Windows;
using Utility.Models;

namespace Utility.Nodify.Core
{
    public class PendingConnectionViewModel : BaseViewModel
    {
        private ConnectorViewModel _source = default!, _target;
        private bool _isVisible;
        private Point _targetLocation;

        public ConnectorViewModel Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        public ConnectorViewModel? Target
        {
            get => _target;
            set => Set(ref _target, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        public Point TargetLocation
        {
            get => _targetLocation;
            set => Set(ref _targetLocation, value);
        }
    }
}
