using System;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class YesNoToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //switch (value.ToString().ToLower())
            //{
            //    case "yes":
            //    case "oui":
            //        return true;
            //    case "no":
            //    case "non":
            //        return false;
            //}
            //return false;

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object x;
            try
            {
                x = ((dynamic)value).Value;
            }
            catch
            {
                return value;
            }
            return x;
        }
    }
}