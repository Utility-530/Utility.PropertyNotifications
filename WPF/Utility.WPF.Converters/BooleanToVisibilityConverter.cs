using System;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public bool Collapse { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a boolean");
            if (Invert)
                return (bool)value ? Collapse ? Visibility.Collapsed : Visibility.Hidden : Visibility.Visible;
            else
                return (bool)value ? Visibility.Visible : Collapse ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members

        public static BooleanToVisibilityConverter Instance { get; } = new();
    }
}