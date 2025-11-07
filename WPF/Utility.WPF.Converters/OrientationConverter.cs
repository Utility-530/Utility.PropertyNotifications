using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.Enums;
using O = System.Windows.Controls.Orientation;

namespace Utility.WPF.Converters
{
    public class OrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Orientation o)
            {
                switch (o)
                {
                    case Orientation.Horizontal:
                        return O.Horizontal;

                    case Orientation.Vertical:
                        return O.Vertical;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}