using System;
using System.Windows;
using Utility.Models;

namespace Utility.Nodify.Core
{
    public class BaseNodeViewModel : BaseViewModel
    {
        public event Action? Closed;

        private bool _isVisible; 
        private Point _location;
        private Size _size; 
        private string? _title;
        private bool _isActive;
        private bool _isSelected;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                Set(ref _isVisible, value);
                if (!value)
                {
                    Closed?.Invoke();
                }
            }
        }

        public Point Location
        {
            get => _location;
            set => Set(ref _location, value);
        }


        public Size Size
        {
            get => _size;
            set => Set(ref _size, value);
        }


        public string? Title
        {
            get => _title;
            set => Set(ref _title, value);
        }


        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }


        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }

        public bool IsReadOnly { get; set; }
    }
}
