using Nodify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static PointConverter Instance { get; } = new PointConverter();
    }

    internal class ArrangementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if(value is Enum)
            //return Enum.Parse<Arrangement>(value.ToString());
            throw new Exception("£%GG%%ff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("£%GG%%ff");
        }
    }

    internal class DirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                if (b)
                    return ConnectionDirection.Forward;
                return ConnectionDirection.Backward;
            }
            throw new Exception("G%%ff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("£%GG%%ff");
        }
    }
}
