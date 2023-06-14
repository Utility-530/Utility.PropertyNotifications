using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Controls.Date.Helper
{
    public class MonthNumberToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default(DateTime).AddMonths((int)value - 1)
                .ToString(parameter?.ToString() ?? "MMMM", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
