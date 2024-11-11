using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Utility.Trees.Demo.Two;

namespace Utility.Trees.Demo.Connections
{
    public class DirectionBrushConverter : IValueConverter
    {

        public DirectionBrushConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Direction direction)
                switch (direction)
                {
                    case Direction.EndToStart:
                        return Brushes.Cyan;
                    case Direction.StartToEnd:
                        return Brushes.Magenta;
                }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
