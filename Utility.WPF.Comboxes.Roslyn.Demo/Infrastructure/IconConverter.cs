using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Utility.WPF.ComboBoxes.Roslyn.Demo
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CommonTypes.Integer)
                return Application.Current.FindResource("Integer") as Path;
            if (value is CommonTypes.Boolean)
                return Application.Current.FindResource("Boolean") as Path;
            if (value is CommonTypes.Double)
                return Application.Current.FindResource("Double") as Path;
            if (value is CommonTypes.String)
                return Application.Current.FindResource("String") as Path;
            if (value is CommonTypes.DateTime)
                return Application.Current.FindResource("DateTime") as Path;
            if (value is CommonTypes.All)
                return Application.Current.FindResource("Cancel") as Path;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
