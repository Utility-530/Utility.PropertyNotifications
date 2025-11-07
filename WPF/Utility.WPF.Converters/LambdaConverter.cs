using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Humanizer;

namespace Utility.WPF.Converters
{
    public static class LambdaConverter
    {
        public static IValueConverter PathToNameConverter => LambdaConverters.ValueConverter.Create<string, string>(e =>
             {
                 return System.IO.Path.GetFileNameWithoutExtension(e.Value);
             });

        public static IValueConverter SecondsToDateTimeConverter => LambdaConverters.ValueConverter.Create<double, DateTime>(e =>
            {
                return DateTime.UnixEpoch + TimeSpan.FromSeconds(e.Value);
            });

        public static IValueConverter DaysToDateTimeConverter => LambdaConverters.ValueConverter.Create<double, DateTime>(e =>
            {
                return DateTime.UnixEpoch + TimeSpan.FromDays(e.Value);
            });

        public static IValueConverter HumanizerConverter =>
            LambdaConverters.ValueConverter.Create<string, string>(a => a.Value?.Humanize() ?? "_null_");

        public static IValueConverter StringConverter =>
     LambdaConverters.ValueConverter.Create<object, string>(a => a.Value?.ToString() ?? "_null_");

        public static IValueConverter CountConverter =>
            LambdaConverters.ValueConverter.Create<int, bool>(a => a.Value != 0);

        public static IValueConverter NoCountConverter =>
            LambdaConverters.ValueConverter.Create<int, bool>(a => a.Value == 0);

        public static IValueConverter BoolToIntConverter =>
            LambdaConverters.ValueConverter.Create<bool, int>(a => System.Convert.ToInt32(a.Value));

        public static IValueConverter MultiplyConverter =>
            LambdaConverters.ValueConverter.Create<double, double, int>(a =>
                a.Parameter * a.Value);

        public static IValueConverter CountToVisibilityConverter =>
            LambdaConverters.ValueConverter.Create<int, Visibility>(a => a.Value != 0 ? Visibility.Visible : Visibility.Collapsed);

        public static IValueConverter NoCountToVisibilityConverter =>
            LambdaConverters.ValueConverter.Create<int, Visibility>(a => a.Value == 0 ? Visibility.Visible : Visibility.Collapsed);

        public static IValueConverter ColorConverter =>
            LambdaConverters.ValueConverter.Create<System.Drawing.Color, Color>(color => Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B));
    }
}