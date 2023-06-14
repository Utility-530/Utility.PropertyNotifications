using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Controls.Trees
{
    public class IndentConverter : IValueConverter
    {
        private const int IndentSize = 16;  // hard-coded into the XAML template

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength(((GridLength)value).Value + IndentSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}