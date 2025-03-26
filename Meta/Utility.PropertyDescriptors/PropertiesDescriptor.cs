
using Utility.Meta;

namespace Utility.PropertyDescriptors
{
    internal record PropertiesDescriptor<T, TR>(string Name) : PropertiesDescriptor(new RootDescriptor(typeof(T), typeof(TR), Name), null)
    {
    }

    internal record PropertiesDescriptor<T>(string Name) : PropertiesDescriptor(new RootDescriptor(typeof(T), null, Name), null)
    {
    }


    public record PropertiesDescriptor(Descriptor Descriptor, object Instance) : BasePropertyDescriptor(Descriptor, Instance), IPropertiesDescriptor, IGetType
    {
        public static string _Name => "Properties";
        public override string? Name => _Name;

        public override IEnumerable<object> Children
        {
            get
            {
                var descriptors = TypeDescriptor.GetProperties(Instance);

                foreach (Descriptor descriptor in descriptors)
                {
                    var propertyDescriptor = DescriptorConverter.ToDescriptor(Instance, descriptor);
                    yield return propertyDescriptor;
                }
            }
        }
        public new Type GetType()
        {
            if (ParentType == null)
            {
                Type[] typeArguments = { Type };
                Type genericType = typeof(PropertiesDescriptor<>).MakeGenericType(typeArguments);
                return genericType;
            }
            else
            {
                Type[] typeArguments = { Type, ParentType };
                Type genericType = typeof(PropertiesDescriptor<,>).MakeGenericType(typeArguments);
                return genericType;
            }
        }
    }
}