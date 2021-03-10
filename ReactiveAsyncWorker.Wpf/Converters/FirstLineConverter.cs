using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace ReactiveAsyncWorker.Wpf.Converters
{
    public class FirstLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                value = Regex.Replace((string)value, @"^\s+$[\r\n\t]*", string.Empty, RegexOptions.Multiline);
                var reader = new StringReader((string)value);
                string first = "";
                while (first == "" && (first = reader.ReadLine()) != null)
                { }
                return first;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}