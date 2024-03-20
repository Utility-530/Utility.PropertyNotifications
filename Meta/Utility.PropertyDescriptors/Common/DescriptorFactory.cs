

using Bogus.DataSets;
using Splat;
using System.Reflection;
using Utility.Interfaces.NonGeneric;

namespace Utility.Descriptors
{
    public class DescriptorFactory //: IValueConverter
    {
        private static ITreeRepository repo => Locator.Current.GetService<ITreeRepository>();


        public static async Task<PropertyDescriptor> ToValue(object? value, Descriptor descriptor, Guid parentGuid)
        {
            var guid = await repo.Find(parentGuid, descriptor.Name, descriptor.PropertyType);

            PropertyDescriptor _descriptor = ToDescriptor(value, descriptor);
            _descriptor.Guid = guid;
            _descriptor.ParentGuid = parentGuid;
            return _descriptor;
        }


        public static async Task<PropertyDescriptor> CreateRoot(Descriptor descriptor, Guid guid)
        {
            await repo.CreateRootKey(guid, descriptor.Name, descriptor.PropertyType);      
            var _descriptor = ToDescriptor(default, descriptor);
            _descriptor.Guid = guid;
            return _descriptor;
        }

        private static PropertyDescriptor ToDescriptor(object? value, Descriptor descriptor)
        {
            PropertyDescriptor _descriptor = descriptor.PropertyType switch
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


                Type t when t.IsDerivedFrom<object>() => new ObjectValue(descriptor, value),

                _ => new NullValue(descriptor, value),

            };
            return _descriptor;
        }



        public static async Task<PropertyDescriptor> CreateItem(object item, int index, Type type, Type parentType, Guid parentGuid)
        {
            PropertyDescriptor descriptor;
            if (type.IsValueOrString())
            {
                descriptor = new CollectionItemDescriptor(item, index, parentType);
            }
            else
            {
                descriptor = new CollectionItemReferenceDescriptor(item, index, parentType);
            }
            var _guid = await repo.Find(parentGuid, type.Name, type, index);
            descriptor.Guid = _guid;
            descriptor.ParentGuid = parentGuid;
            return descriptor;
        }


        public static async Task<MemberDescriptor> CreateMethodItem(object item, MethodInfo methodInfo, Type type, Guid parentGuid)
        {
            var descriptor = new MethodDescriptor(methodInfo, item);
            var _guid = await repo.Find(parentGuid, type.Name);
            descriptor.Guid = _guid;
            descriptor.ParentGuid = parentGuid;
            return descriptor;
        }
    }


}
