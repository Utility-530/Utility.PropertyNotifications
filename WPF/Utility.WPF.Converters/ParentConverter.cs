using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.WPF.Converters
{
    public class ParentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FrameworkElement child)
            {
                if (parameter is int i)
                {
                    if (i == 0)
                    {
                        return VisualTreeHelper.GetParent(child);
                    }
                    else
                    {
                        return Convert(VisualTreeHelper.GetParent(child), targetType, i - 1, culture);
                    }
                }
                if (int.TryParse(parameter.ToString(), out var i2))
                {
                    if (i2 == 0)
                    {
                        return VisualTreeHelper.GetParent(child);
                    }
                    else
                    {
                        return Convert(VisualTreeHelper.GetParent(child), targetType, i2 - 1, culture);
                    }
                }
                else
                {
                    return VisualTreeHelper.GetParent(child);
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ParentConverter Instance { get; } = new();
    }
}