using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class StringAbbreviationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            StringBuilder sb = new();
            if (value is string _string)
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

                if (_length.HasValue == false)
                    sb.Append(_string);
                else
                    sb.Append(_string.AsSpan(0, _length.Value));

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
