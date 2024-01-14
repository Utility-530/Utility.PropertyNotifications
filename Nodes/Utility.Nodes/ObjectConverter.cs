using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

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
                object _value => new DefaultValue(a, value),
                null => new NullValue(),

            }; ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        //public DataTemplate UnknownTemplate { get; set; }
    }


    public class DefaultValue : BasePropertyObject, IValue
    {
        public DefaultValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public object Value => Descriptor.GetValue(Instance);

        public override bool IsReadOnly=>true;
    }


    public class StringValue : BasePropertyObject, IValue<string>
    {

        public StringValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {
        }

        public string Value { get => (string)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); } }

        object IValue.Value => Value;
    }

    public class IntegerValue : BasePropertyObject, IValue<int>
    {
        public IntegerValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }
        public int Value
        {
            get => (int)Descriptor.GetValue(Instance); set
            {
                Descriptor.SetValue(Instance, value);
            }
        }

        object IValue.Value => Value;
    }

    public class LongValue : BasePropertyObject, IValue<long>
    {
        public LongValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }
        public long Value
        {
            get => (long)Descriptor.GetValue(Instance); set
            {
                Descriptor.SetValue(Instance, value);
            }
        }

        object IValue.Value => Value;
    }

    public class EnumValue : BasePropertyObject, IValue<Enum>
    {


        public EnumValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public Enum Value
        {
            get => (Enum)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }

        }

        object IValue.Value => Value;
    }

    public class BooleanValue : BasePropertyObject, IValue<bool>
    {

        public BooleanValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {


        }

        public bool Value
        {
            get => (bool)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }

        object IValue.Value => Value;
    }

    public class DoubleValue : BasePropertyObject, IValue<double>
    {

        public DoubleValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {


        }

        public double Value
        {
            get => (double)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }


        object IValue.Value => Value;
    }

    public class GuidValue : BasePropertyObject, IValue<Guid>
    {


        public GuidValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public Guid Value
        {
            get => (Guid)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }


        object IValue.Value => Value;
    }

    public class DateTimeValue : BasePropertyObject, IValue<DateTime>
    {


        public DateTimeValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public DateTime Value
        {
            get => (DateTime)Descriptor.GetValue(Instance); set { Descriptor.SetValue(Instance, value); }
        }


        object IValue.Value => Value;
    }

    public record NullValue() : IValue
    {
        object? IValue.Value => null;
    }


    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

}
