using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers;

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
                var flags = EnumHelper.GetFlags(propertyType);
                if (flags.Contains(PropertyType.GenericArgument1))
                    type = type.GetGenericArguments().First();

                else if (flags.Contains(PropertyType.GenericArgument2))
                    type = type.GetGenericArguments().Skip(1).First();


                if (flags.Contains(PropertyType.TopLevel))
                    return Utility.Helpers.PropertyHelper.TopLevelPublicInstanceProperties(type).Select(a => a.Name).ToArray();
                else
                    return Utility.Helpers.PropertyHelper.PublicInstanceProperties(type).Select(a => a.Name);
            }
            return Utility.Helpers.PropertyHelper.PublicInstanceProperties(type).Select(a => a.Name);
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
