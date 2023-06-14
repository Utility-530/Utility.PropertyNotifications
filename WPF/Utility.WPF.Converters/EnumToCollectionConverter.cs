using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using static Utility.Helpers.EnumHelper;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetAllValuesAndDescriptions(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static EnumToCollectionConverter Instance { get; } = new EnumToCollectionConverter();
    }
}