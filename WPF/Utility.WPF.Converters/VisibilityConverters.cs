using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class BooleanConverter<T> : BaseConverter<T>
    {
        public BooleanConverter(T trueValue, T falseValue) : base(trueValue, falseValue)
        {
        }

        protected override bool Check(object value)
        {
            return value is true;
        }
    }

    //public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    //{
    //    static BooleanToVisibilityConverter()
    //    {
    //    }

    //    public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed)
    //    { }

    //    public static BooleanToVisibilityConverter Instance => new BooleanToVisibilityConverter();
    //}

    public sealed class InvertedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        static InvertedBooleanToVisibilityConverter()
        {
        }

        public InvertedBooleanToVisibilityConverter() : base(Visibility.Collapsed, Visibility.Visible)
        { }

        public static BooleanToVisibilityConverter Instance => new BooleanToVisibilityConverter();
    }

    public sealed class InvertedBooleanHiddenToVisibilityConverter : BooleanConverter<Visibility>
    {
        static InvertedBooleanHiddenToVisibilityConverter()
        {
        }

        public InvertedBooleanHiddenToVisibilityConverter() : base(Visibility.Visible, Visibility.Hidden)
        { }

        public static InvertedBooleanHiddenToVisibilityConverter Instance => new InvertedBooleanHiddenToVisibilityConverter();
    }

    public sealed class StringToVisibilityConverter : BaseConverter<Visibility>
    {
        static StringToVisibilityConverter()
        {
        }

        public StringToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }

        protected override bool Check(object value)
        {
            return string.IsNullOrEmpty((string)value) == false;
        }

        public static BooleanToVisibilityConverter Instance => new();
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
        public static HasDataTemplateToVisibilityConverter Instance => new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dataTemplate = Application.Current.Resources[value];
            return dataTemplate is DataTemplate ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}