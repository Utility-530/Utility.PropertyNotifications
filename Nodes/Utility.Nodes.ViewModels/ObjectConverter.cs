using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using Utility.Interfaces.Generic;

namespace Utility.Nodes
{
    public class ObjectConverter //: IValueConverter
    {


        public static object ToValue(object value, PropertyDescriptor a)
        {

            return a.GetValue(value) switch
            {
                string _value => new StringValue(a, value),
                Enum _value => new EnumValue(a, value),
                bool _value => new BooleanValue(a, value),
                int _value => new IntegerValue(a, value),
                long _value => new LongValue(a, value),
                double _value => new DoubleValue(a, value),
                Guid guid => new GuidValue(a, value),
                DateTime dateTime => new DateTimeValue(a, value),
                IDictionary dictionary => new DictionaryValue(a, value),
                object _value => new DefaultValue(a, value),
                null => new NullValue(a, value),

            }; ; ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public DataTemplate UnknownTemplate { get; set; }
    }

    public class DictionaryValue : BasePropertyObject, IValue<IDictionary>
    {
        public DictionaryValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public IDictionary Value => default(IDictionary);

        object Interfaces.NonGeneric.IValue.Value => this.Value;
    }

}
