using Splat;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Reactives;
using Utility.Repos;

namespace Utility.Descriptors
{
    public class DescriptorFactory //: IValueConverter
    {
        private static ITreeRepository repo => Locator.Current.GetService<ITreeRepository>();

        public static IObservable<ValuePropertyDescriptor> ToValue(object? value, Descriptor descriptor, Guid parentGuid)
        {
            return
                Observable.Create<ValuePropertyDescriptor>(observer =>
                {
                    return repo.Find(parentGuid, descriptor.Name, descriptor.PropertyType)
                    .Subscribe(guid =>
                    {
                        ValuePropertyDescriptor _descriptor = DescriptorConverter.ToDescriptor(value, descriptor);
                        _descriptor.Guid = guid;
                        _descriptor.ParentGuid = parentGuid;
                        observer.OnNext(_descriptor);
                    });
                });
        }


        public static IObservable<IValueDescriptor> CreateRoot(Type type, Guid guid, string? name = null)
        {
            var instance = Activator.CreateInstance(type);
            var rootDescriptor = new RootDescriptor(type, name: name);
            rootDescriptor.SetValue(null, instance);
            var root = CreateRoot(rootDescriptor, guid);
            //root.Initialise();
            return root;

            static IObservable<IValueDescriptor> CreateRoot(Descriptor descriptor, Guid guid)
            {
                return Observable.Create<IValueDescriptor>(obs =>
                {
                    return repo.InsertRoot(guid, descriptor.Name, descriptor.PropertyType)
                        .Subscribe(key =>
                        {
                            var _descriptor = DescriptorConverter.ToDescriptor(default, descriptor);
                            _descriptor.Guid = key.Guid;

                            obs.OnNext(_descriptor);
                        });
                });
            }
        }

        public static IObservable<IValueDescriptor> CreateRoot(object instance, Guid guid, string? name = null)
        {
            var rootDescriptor = new RootDescriptor(instance.GetType(), name: name);
            rootDescriptor.SetValue(null, instance);
            var root = CreateRoot(rootDescriptor, guid);
            //root.Initialise();
            return root;

            static IObservable<IValueDescriptor> CreateRoot(Descriptor descriptor, Guid guid)
            {
                return Observable.Create<IValueDescriptor>(obs =>
                {
                    return repo.InsertRoot(guid, descriptor.Name, descriptor.PropertyType)
                        .Subscribe(key =>
                        {
                            var _descriptor = DescriptorConverter.ToDescriptor(default, descriptor);
                            _descriptor.Guid = key.Guid;

                            obs.OnNext(_descriptor);
                        });
                });
            }
        }

        public static IObservable<IDescriptor> CreateItem(object item, int index, Type type, Type parentType, Guid parentGuid)
        {
            MemberDescriptor descriptor;
            if (type.IsValueOrString())
            {
                descriptor = new CollectionItemDescriptor(item, index, parentType);
            }
            else
            {
                descriptor = new CollectionItemReferenceDescriptor(item, index, parentType);
            }
            return Observable.Create<IDescriptor>(observer =>
            {
                return repo.Find(parentGuid, type.Name, type, index).Subscribe(_guid =>
                {

                    descriptor.Guid = _guid;
                    descriptor.ParentGuid = parentGuid;
                    observer.OnNext( descriptor);
                });
            });
        }

        public static async Task<IDescriptor> CreateMethodItem(object item, MethodInfo methodInfo, Type type, Guid parentGuid)
        {
            var descriptor = new MethodDescriptor(methodInfo, item);
            var _guid = await repo.Find(parentGuid, type.Name).ToTask();
            descriptor.Guid = _guid;
            descriptor.ParentGuid = parentGuid;
            return descriptor;
        }
    }

    class DescriptorConverter
    {
        public static ValuePropertyDescriptor ToDescriptor(object? value, Descriptor descriptor)
        {
            ValuePropertyDescriptor _descriptor = descriptor.PropertyType switch
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


                Type t when t.IsDerivedFrom<object>() => new ReferenceDescriptor(descriptor, value),

                _ => new NullValue(descriptor, value),

            };
            return _descriptor;
        }
    }
}
