using Descriptor = System.ComponentModel.PropertyDescriptor;
using Utility.Changes;
using System.Reactive.Linq;

namespace Utility.PropertyDescriptors
{
    public record PropertiesDescriptor(Descriptor PropertyDescriptor, object Instance) : PropertyDescriptor(PropertyDescriptor, Instance), IPropertiesDescriptor
    {
        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            return ChildPropertyExplorer.Explore(Instance, this);
        }
    }
}


