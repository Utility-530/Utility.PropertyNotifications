using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF.ComboBoxes.Roslyn
{
    public class IndexItemConverter : IMultiValueConverter
    {
        public int Index { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ItemsControl itemscontrol = values[0] as ItemsControl;
            int count = itemscontrol.Items.Count;
            int index = Index;
            if (values != null && values.Length >= 2 && count > 0)
            {
                if (values.Length == 3 && values[2] is int i)
                {
                    index = i;
                }
                var itemContext = (values[1] as FrameworkElement).DataContext;
                var item = itemscontrol.ItemsSource.ElementAt(index);
                return Equals(item, itemContext);
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
