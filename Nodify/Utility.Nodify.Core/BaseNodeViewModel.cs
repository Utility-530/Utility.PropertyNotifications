using System;
using System.Drawing;
using System.Windows;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Base
{
    public class BaseNodeViewModel : NotifyPropertyClass, IKey
    {
        public event Action? Closed;

        private bool _isVisible;
        private PointF _location;
        private SizeF _size;
        private string? _title;
        private bool _isActive;
        private bool _isSelected;

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
    }
}
