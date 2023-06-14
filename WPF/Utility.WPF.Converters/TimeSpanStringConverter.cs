using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class TimeSpanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                TimeSpan span = (TimeSpan)value;
                string formatted = string.Format("{0}{1}{2}{3}",
            span.Duration().Days > 0 ? $"{span.Days:0} day{(span.Days == 1 ? string.Empty : "s")}, " : string.Empty,
            span.Duration().Hours > 0 ? $"{span.Hours:0} hour{(span.Hours == 1 ? string.Empty : "s")}, " : string.Empty,
            span.Duration().Minutes > 0 ? $"{span.Minutes:0} minute{(span.Minutes == 1 ? string.Empty : "s")}, " : string.Empty,
            span.Duration().Seconds > 0 ? $"{span.Seconds:0} second{(span.Seconds == 1 ? string.Empty : "s")}" : string.Empty);

                if (formatted.EndsWith(", ")) formatted = formatted[..^2];

                if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

                return formatted;
            }
            else { return ""; }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is DateTime & values[1] is DateTime)
            {
                var ts = (DateTime)values[0] - (DateTime)values[1];

                switch (ts.Days)
                {
                    case (0):
                        return $"{ts.Hours}:{ts.Minutes}:{ts.Seconds}";

                    case (1):
                        return $"1 day {ts.Hours} hours";

                    default:
                        if (ts.Days > 1)
                            return $"{ts.Days} days {ts.Hours}:{ts.Minutes}:{ts.Seconds}";
                        else
                            return ((DateTime)values[0]).ToString("D", CultureInfo.CurrentCulture);
                }
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}