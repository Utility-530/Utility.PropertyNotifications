using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class TypeHierarchyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string s && Type.GetType(s) is Type _type)
            {
                return new string[] { _type.Assembly.GetName().Name, _type.Namespace, _type.Name };
            }
            if(value is Type type)
            {
                return new string[] { type.Assembly.GetName().Name, type.Namespace, type.Name };
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static TypeHierarchyConverter Instance { get; } = new();
    }
}
