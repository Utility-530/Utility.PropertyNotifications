
namespace Utility.Descriptors
{
    public record PropertiesDescriptor(Descriptor PropertyDescriptor, object Instance) : PropertyDescriptor(PropertyDescriptor, Instance), IPropertiesDescriptor
    {
        public static string _Name => "Properties";
        public override string? Name => _Name;
        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            return ChildPropertyExplorer.Explore(Instance, this);
        }
    }
}


