using System;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public sealed class PathToNameConverter : IValueConverter
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PathToNameConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ResourceDictionary resourceDictionary)
                return System.IO.Path.GetFileNameWithoutExtension(resourceDictionary.Source.OriginalString);
            else if (value is Uri uri)
                return System.IO.Path.GetFileNameWithoutExtension(uri.OriginalString);
            else if (value is string path)
                return System.IO.Path.GetFileNameWithoutExtension(path);

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static PathToNameConverter Instance { get; } = new();
    }
}