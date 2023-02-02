using System;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(int), typeof(int))]
    public class RounderConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int param = System.Convert.ToInt32(parameter);
            int val = System.Convert.ToInt32(value);
            int result = val % param >= param / 2 ? val + param - val % param : val - val % param;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members
    }

    [ValueConversion(typeof(int), typeof(int))]
    public class Rounder2Converter : IValueConverter
    {
        #region IValueConverter Members

        public int Factor { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val = System.Convert.ToInt32(value);

            return (int)(RoundHelper.Round(val) / Factor);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion IValueConverter Members

        private class RoundHelper
        {
            public static int Round(int number)
            {
                double value = 1000000000;

                while (number - value < 0)
                {
                    value /= 10;
                }

                return (int)value;
            }
        }
    }
}