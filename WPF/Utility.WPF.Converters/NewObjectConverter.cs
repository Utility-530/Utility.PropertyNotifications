using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;

namespace Utility.WPF.Converters
{
    public class NewObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IProliferation proliferation)
            {
                var x = proliferation.Proliferation().FirstOrDefault();
                return x;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}