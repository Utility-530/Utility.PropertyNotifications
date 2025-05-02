using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leepfrog.WpfFramework.Converters
{
    public class AnyChangeConverter : IMultiValueConverter
    {
        public AnyChangeConverter()
        {

        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])value;
        }
    }

}
