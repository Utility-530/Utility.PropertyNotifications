using System;
using System.Windows.Data;
using Utility.Helpers;


namespace Utility.WPF.Converters
{
    // Converts enumerable's to a distinct list of given property's (parameter)  value
    //[ValueConversion(typeof(string), typeof(IFilter))]
    public class StringToFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new ContainsFilter((string)value, (string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value ? new object() : null;
        }

        public class ContainsFilter
        {
            private readonly string? a;
            private readonly string property;

            public ContainsFilter(string a)
            {
                this.a = a;
            }

            public ContainsFilter(string a, string? property = null)
            {
                this.property = property;
                this.a = a;
            }

            public bool Filter(object o) => property == null ?
                ((string)o).Contains(a, System.StringComparison.InvariantCultureIgnoreCase) :
                o.GetPropertyRefValue<string>(property)?.Contains(a, System.StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
    }
}