using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Converters
{
    public class PropertyNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            Type type = value.GetType();
            if (parameter is PropertyType propertyType)
            {
                var flags = EnumHelper.SeparateFlags(propertyType);
                if (flags.Contains(PropertyType.GenericArgument1))
                    type = type.GetGenericArguments().FirstOrDefault() is Type gtype ? gtype : type;
                else if (flags.Contains(PropertyType.GenericArgument2))
                    type = type.GetGenericArguments().Skip(1).FirstOrDefault() is Type gtype ? gtype : type;

                if (flags.Contains(PropertyType.TopLevel))
                    return PropertyHelper.TopLevelPublicInstanceProperties(type).Select(a => a.Name).ToArray();
                else
                    return PropertyHelper.PublicInstanceProperties(type).Select(a => a.Name);
            }
            return PropertyHelper.PublicInstanceProperties(type).Select(a => a.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum PropertyType
    {
        Default,
        GenericArgument1,
        GenericArgument2,
        TopLevel,
        TopLevelAndGenericArgument1 = GenericArgument1 | TopLevel,
        TopLevelAndGenericArgument2 = GenericArgument2 | TopLevel,
    }
}