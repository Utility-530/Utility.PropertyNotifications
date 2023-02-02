using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class TimeSpanMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is DateTime & values[1] is DateTime)
            {
                var ts = (DateTime)values[0] - (DateTime)values[1];

                switch (ts.Days)
                {
                    case 0:
                        return $"{ts.Hours }:{ts.Minutes}:{ts.Seconds}";

                    case 1:
                        return $"1 day { ts.Hours } hours";

                    default:
                        if (ts.Days > 1)
                            return $"{ts.Days} days {ts.Hours }:{ts.Minutes}:{ts.Seconds}";
                        else
                            return ((DateTime)values[0]).ToString("D", CultureInfo.CurrentCulture);
                }
            }
            else { return ""; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}