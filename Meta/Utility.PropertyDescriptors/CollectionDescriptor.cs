using Descriptor = System.ComponentModel.PropertyDescriptor;
using Utility.Changes;

namespace Utility.PropertyDescriptors
{
    public record CollectionDescriptor(Descriptor PropertyDescriptor, object Instance) : PropertyDescriptor(PropertyDescriptor, Instance)
    {
        public static string _Name => "Collection";

        public override string? Name => _Name;


        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            return ChildPropertyExplorer.CollectionItemDescriptors(Instance, this);
        }
    }
}


