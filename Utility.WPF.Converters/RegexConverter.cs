using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Color))]
    public class RegexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value is string text ?
                 parameter is string regex ?
                 Helper.Regex(text, regex) :
                 DependencyProperty.UnsetValue :
                 DependencyProperty.UnsetValue;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static RegexConverter Instance { get; } = new RegexConverter();
    }

    public static class Helper
    {
        public static string Regex(string input, string pattern)
        {
            var regex = new Regex(pattern);
            var match = regex.Match(input);
            return match.Groups[1].Captures[0].Value;
        }
    }
}