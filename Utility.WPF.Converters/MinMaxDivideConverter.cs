using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class MinMaxDivideConverter : IMultiValueConverter
    {
        public int Factor { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double min = System.Convert.ToDouble(values[0]);
            double max = System.Convert.ToDouble(values[1]);

            return (int)((max - min) / Factor);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}