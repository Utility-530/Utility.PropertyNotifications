using NetFabric.Hyperlinq;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using Utility.Helpers;

namespace Utility.WPF.Converters
{

    public enum ConverterType
    {
        BooleanToVisibility
    }


    public class Converter : MarkupExtension, IValueConverter
    {
        private ConverterSingleton[] types;
        private Type p;

        public Converter(ConverterType value)
        {
            Value = value;
        }

        public Converter()
        {

        }

        public ConverterType? Value { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (Value switch
            {
                ConverterType.BooleanToVisibility => BooleanToVisibilityConverter.Instance,
                _ => Match(value.GetType(), targetType),
            }).Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public IValueConverter Match(Type valueType, Type targetType)
        {
            p ??= typeof(IValueConverter);
            types = typeof(Converter).Assembly.GetTypes()
                                .Where(type => type.IsAssignableTo(p))
                                .Select(mi => (mi, mi.GetAttributeSafe<ValueConversionAttribute>()))
                                .Where(a => a.Item2.success)
                                .Select(a => new ConverterSingleton(a.mi, a.Item2.attribute))
                                .ToArray();

            return types.SingleOrDefault(a => a.ValueConversionAttribute.TargetType == targetType && a.ValueConversionAttribute.SourceType == valueType)?.Instance;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;


    }

    public record ConverterSingleton(MemberInfo MemberInfo, ValueConversionAttribute ValueConversionAttribute)
    {
        private object? instance;

        public IValueConverter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Activator.CreateInstance(MemberInfo as Type);
                }
                return (IValueConverter)instance;
            }
        }
    }

}
