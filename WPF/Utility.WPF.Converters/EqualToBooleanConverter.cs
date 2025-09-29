using System;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class EqualToBooleanConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter) != Invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static EqualToBooleanConverter Instance => new EqualToBooleanConverter();
        public static EqualToBooleanConverter InverseInstance => new() { Invert = true };
    }


    public class EqualToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter) != Invert ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static EqualToVisibilityConverter Instance => new ();
        public static EqualToVisibilityConverter InverseInstance => new () { Invert=true };
    }   
}