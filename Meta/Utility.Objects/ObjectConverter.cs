using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Utility.Helpers;
using Utility.Interfaces.Generic;
using Utility.Nodes;

namespace Utility.Objects
{
    public class ObjectConverter //: IValueConverter
    {


        public static object ToValue(object value, PropertyDescriptor descriptor)
        {

            return descriptor.PropertyType switch
            {
                Type t when  t==typeof(string) => new StringValue(descriptor, value),
                Type t when t.BaseType == typeof(Enum)  => new EnumValue(descriptor, value),
                Type t when t == typeof(bool)  => new BooleanValue(descriptor, value),
                Type t when t == typeof(int)   => new IntegerValue(descriptor, value),
                Type t when t == typeof(short)   => new IntegerValue(descriptor, value),
                Type t when t == typeof(long)   => new LongValue(descriptor, value),
                Type t when t == typeof(double)  => new DoubleValue(descriptor, value),
                Type t when t == typeof(byte)   => new ByteValue(descriptor, value),
                Type t when t == typeof(Guid)   => new GuidValue(descriptor, value),
                Type t when t == typeof(DateTime)  => new DateTimeValue(descriptor, value),

                Type t when t.IsNullableEnum() => new NullableEnumValue(descriptor, value),
                Type t when t == typeof(bool?) => new NullableBooleanValue(descriptor, value),
                Type t when t == typeof(int?) => new NullableIntegerValue(descriptor, value),
                Type t when t == typeof(short?) => new NullableIntegerValue(descriptor, value),
                Type t when t == typeof(long?) => new NullableLongValue(descriptor, value),
                Type t when t == typeof(double?) => new NullableDoubleValue(descriptor, value),
                Type t when t == typeof(byte?) => new NullableByteValue(descriptor, value),
                Type t when t == typeof(Guid?) => new NullableGuidValue(descriptor, value),
                Type t when t == typeof(DateTime?) => new NullableDateTimeValue(descriptor, value),

                Type t when t == typeof(IDictionary)   => new DictionaryValue(descriptor, value),
                Type t when t.IsDerivedFrom<object>()   => new ObjectValue(descriptor, value),

                _ => new NullValue(descriptor, value),

            }; ; ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public record DictionaryValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData(propertyDescriptor, Instance), IValue<IDictionary>
    {
        public IDictionary Value => default;

        object Interfaces.NonGeneric.IValue.Value => Value;
    }

}
