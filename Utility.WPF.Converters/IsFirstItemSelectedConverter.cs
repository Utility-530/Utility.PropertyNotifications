using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class IsFirstItemSelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value[1] is null || value[1] == DependencyProperty.UnsetValue)
            {
                return true != Invert;
            }
            if (value[0] is ItemsControl && value[1] is int index)
            {
                return (index == -1 || index == 0) != Invert;
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public bool Invert { get; set; }
    }
}