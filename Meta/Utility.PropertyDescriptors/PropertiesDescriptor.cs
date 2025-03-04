
namespace Utility.PropertyDescriptors
{
    public record PropertiesDescriptor(Descriptor PropertyDescriptor, object Instance) : BasePropertyDescriptor(PropertyDescriptor, Instance), IPropertiesDescriptor
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
                    var propertyDescriptor = new PropertyDescriptor(descriptor, Instance) { };
                    yield return propertyDescriptor;
                }
            }
        }
    }
}