using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class BooleanConverter<T>(T trueValue, T falseValue) : BaseConverter<T>(trueValue, falseValue)
    {
        protected override bool Check(object value)
        {
            return value is true;
        }
    }

    public sealed class InvertedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InvertedBooleanToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Visible)
        { }

        public static InvertedBooleanToVisibilityConverter Instance => new();
    }

    public sealed class InvertedBooleanHiddenToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InvertedBooleanHiddenToVisibilityConverter() : base(Visibility.Hidden, Visibility.Visible)
        { }

        public static InvertedBooleanHiddenToVisibilityConverter Instance => new();
    }

    public sealed class StringToVisibilityConverter : BaseConverter<Visibility>
    {
        public StringToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }

        protected override bool Check(object value)
        {
            return string.IsNullOrEmpty((string)value) == false;
        }

        public static StringToVisibilityConverter Instance => new();
    }

    public class NullToVisibilityConverter : BaseConverter<Visibility>
    {
        public NullToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed)
        { }

        protected override bool Check(object value) => value == null;

        public static NullToVisibilityConverter Instance => new();
    }

    public class NullToInverseVisibilityConverter : BaseConverter<Visibility>
    {
        public NullToInverseVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed)
        { }

        protected override bool Check(object value) => value != null;

        public static NullToInverseVisibilityConverter Instance => new();
    }

    public class HasDataTemplateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataTemplate = Application.Current.Resources[value];
            return dataTemplate is DataTemplate ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static HasDataTemplateToVisibilityConverter Instance => new();
    }
}