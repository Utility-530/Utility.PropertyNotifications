using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class PositionItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ItemsControl itemscontrol = values[0] as ItemsControl;
            int count = itemscontrol.Items.Count;
            int position = 0;
            try
            {
                position = System.Convert.ToInt32(values[2]);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
            if (values != null && values.Length == 3 && count > 0 && position >= 0 && position < count)
            {
                var itemContext = (values[1] as ContentPresenter).DataContext;
                var lastItem = itemscontrol.Items[position];
                return Equals(lastItem, itemContext);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}