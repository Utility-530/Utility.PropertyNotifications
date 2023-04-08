using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class GravatarConverter : IValueConverter
    {

        HandyControl.Tools.GithubGravatarGenerator generator = new HandyControl.Tools.GithubGravatarGenerator();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return generator.GetGravatar(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static GravatarConverter Instance { get; } = new();
    }
}
