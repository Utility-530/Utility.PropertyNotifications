using Utility.Meta;

namespace Utility.PropertyDescriptors
{
    public class DescriptorFactory //: IValueConverter
    {
        public static IDescriptor CreateRoot(Type type, string? name = null)
        {
            var instance = Activator.CreateInstance(type);
            var rootDescriptor = new RootDescriptor(type, name: name);
            rootDescriptor.SetValue(null, instance);
            var root = DescriptorConverter.ToDescriptor(instance, rootDescriptor);
            //root.Initialise();
            return root;
        }

        public static IDescriptor CreateRoot(object instance, string? name = null)
        {
            var rootDescriptor = new RootDescriptor(instance.GetType(), name: name);
            rootDescriptor.SetValue(null, instance);
            return DescriptorConverter.ToDescriptor(instance, rootDescriptor);
        }

        public static IDescriptor CreateMethodItem(object item, MethodInfo methodInfo, Type type)
        {
            var descriptor = new MethodDescriptor(methodInfo, item);
            return descriptor;
        }
    }

    public class DescriptorConverter
    {
        public static MemberDescriptor ToDescriptor(object? value, Descriptor descriptor)
        {
            MemberDescriptor _descriptor = descriptor.PropertyType switch
            {
                Type t when t == typeof(string) => new StringValue(descriptor, value),
                Type t when t.BaseType == typeof(Enum) => new EnumValue(descriptor, value),
                Type t when t == typeof(bool) => new BooleanValue(descriptor, value),
                Type t when t == typeof(int) => new IntegerValue(descriptor, value),
                Type t when t == typeof(short) => new IntegerValue(descriptor, value),
                Type t when t == typeof(long) => new LongValue(descriptor, value),
                Type t when t == typeof(double) => new DoubleValue(descriptor, value),
                Type t when t == typeof(byte) => new ByteValue(descriptor, value),
                Type t when t == typeof(Guid) => new GuidValue(descriptor, value),
                Type t when t == typeof(DateTime) => new DateTimeValue(descriptor, value),
                Type t when t == typeof(Type) => new TypeValue(descriptor, value),

                Type t when t.IsNullableEnum() => new NullableEnumValue(descriptor, value),
                Type t when t == typeof(bool?) => new NullableBooleanValue(descriptor, value),
                Type t when t == typeof(int?) => new NullableIntegerValue(descriptor, value),
                Type t when t == typeof(short?) => new NullableIntegerValue(descriptor, value),
                Type t when t == typeof(long?) => new NullableLongValue(descriptor, value),
                Type t when t == typeof(double?) => new NullableDoubleValue(descriptor, value),
                Type t when t == typeof(byte?) => new NullableByteValue(descriptor, value),
                Type t when t == typeof(Guid?) => new NullableGuidValue(descriptor, value),
                Type t when t == typeof(DateTime?) => new NullableDateTimeValue(descriptor, value),

                //Type t when t == typeof(IDictionary) => new DictionaryValue(descriptor, value),
                Type t when t.IsValueType => new StructValue(descriptor, value),


                Type t when t.IsDerivedFrom<object>() && tryGetValue(descriptor, value, out var _value) => new ReferenceDescriptor(descriptor, _value),

                _ => new NullValue(descriptor, value),

            };

            //_descriptor.Initialise();
            return _descriptor;
        }

        static bool tryGetValue(Descriptor descriptor, object instance, out object value)
        {
            try
            {
                value = descriptor.GetValue(instance);
                return true;
            }
            catch(Exception ex)
            {
                value = ex;
                return false;
            }
        }
    }
}
