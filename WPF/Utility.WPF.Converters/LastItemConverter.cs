using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class LastItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ItemsControl itemscontrol && (values[1] is FrameworkElement fe))
            {
                int count = itemscontrol.Items.Count;

                if (values != null && values.Length >= 2 && count > 0)
                {
                    var lastItem = itemscontrol.Items[count - 1];
                    return Equals(lastItem, fe.DataContext);
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}