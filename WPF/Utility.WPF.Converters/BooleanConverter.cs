using System;
using System.Globalization;

namespace Utility.WPF.Converters
{
    public class BooleanConverter : BaseConverter<bool>
    {
        protected BooleanConverter() : base(true, false)
        { }

        protected override bool Check(object value) => System.Convert.ToBoolean(value);
    }

    public class InverseBooleanConverter : BooleanConverter
    {
        protected override bool Check(object value) => !System.Convert.ToBoolean(value);

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Check(value);
        }

        public static InverseBooleanConverter Instance => new();
    }

    //public class NullToBooleanConverter : BooleanConverter
    //{
    //    protected override bool Check(object value) => value == null;

    //    public static NullToBooleanConverter Instance => new();
    //}

    //public class NullToInverseBooleanConverter : BooleanConverter
    //{
    //    protected override bool Check(object value) => value != null;

    //    public static NullToInverseBooleanConverter Instance => new();
    //}

    public class NullOrEmptyToInverseBooleanConverter : BooleanConverter
    {
        protected override bool Check(object value) => !string.IsNullOrEmpty(value.ToString());

        public static NullOrEmptyToInverseBooleanConverter Instance => new();
    }

    public class NullOrEmptyToBooleanConverter : BooleanConverter
    {
        protected override bool Check(object value) => string.IsNullOrEmpty(value.ToString());

        public static NullOrEmptyToBooleanConverter Instance => new();
    }
}