using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Utility.Nodify.Engine.Infrastructure
{
    internal class PointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PointF { X: { } x, Y: { } y } && targetType == typeof(System.Windows.Point))
            {
                return new System.Windows.Point(x, y);
            }
            throw new Exception("£%GG%%ff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Point { X: { } x, Y: { } y })
            {
                return new PointF((float)x, (float)y);
            }
            throw new Exception("£%GG%%ff");
        }
    }
}
