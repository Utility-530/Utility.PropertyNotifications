using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class CollectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder sb = new();
            if (value is IEnumerable enumerable)
            {
                int? _length = 0;
                if (parameter is int length)
                {
                    _length = length;
                }
                else if (parameter is string sLength)
                {
                    _length = int.Parse(sLength);
                }
                var enumerator = enumerable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (_length.HasValue == false)
                        sb.Append(enumerator.Current.ToString());
                    else
                        sb.Append(enumerator.Current.ToString().Substring(0, _length.Value));
                    sb.Append(',');
                }
                return sb.ToString();
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}