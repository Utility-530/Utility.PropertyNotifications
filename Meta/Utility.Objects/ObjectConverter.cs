using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Utility.Interfaces.Generic;
using Utility.Nodes;

namespace Utility.Objects
{
    public class ObjectConverter //: IValueConverter
    {


        public static object ToValue(object value, PropertyDescriptor descriptor)
        {

            return descriptor.GetValue(value) switch
            {
                string _value => new StringValue(descriptor, value),
                Enum _value => new EnumValue(descriptor, value),
                bool _value => new BooleanValue(descriptor, value),
                int _value => new IntegerValue(descriptor, value),
                long _value => new LongValue(descriptor, value),
                double _value => new DoubleValue(descriptor, value),
                Guid guid => new GuidValue(descriptor, value),
                DateTime dateTime => new DateTimeValue(descriptor, value),
                IDictionary dictionary => new DictionaryValue(descriptor, value),
                object _value => new ObjectValue(descriptor, value),
                null => new NullValue(descriptor, value),

            }; ; ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public DataTemplate UnknownTemplate { get; set; }
    }

    public record DictionaryValue(PropertyDescriptor propertyDescriptor, object Instance) : PropertyData(propertyDescriptor, Instance), IValue<IDictionary>
    {
        public IDictionary Value => default;

        object Interfaces.NonGeneric.IValue.Value => Value;
    }

}
