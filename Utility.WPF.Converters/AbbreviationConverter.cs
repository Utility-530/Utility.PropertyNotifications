using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters;

public class AbbreviationConverter : IValueConverter
{
    public int MaxLength { get; set; } = 3;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string val)
            return DependencyProperty.UnsetValue;

        var length = parameter is int param ? param : MaxLength;
        var arr = val.ToCharArray();
        if (arr.Length > length)
            return new string(arr.Take(MaxLength).ToArray()) + "...";
        return val;
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
       => throw new NotImplementedException();

    public static AbbreviationConverter Instance { get; } = new();
}
