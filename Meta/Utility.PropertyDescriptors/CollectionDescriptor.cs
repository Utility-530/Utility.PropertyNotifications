

using Utility.Helpers.NonGeneric;

namespace Utility.Descriptors
{
    public record CollectionDescriptor(Descriptor PropertyDescriptor, object Instance) : PropertyDescriptor(PropertyDescriptor, Instance), ICount
    {
        public static string _Name => "Collection";

        public override string? Name => _Name;


        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            return ChildPropertyExplorer.CollectionItemDescriptors(Instance, this);
        }

        // collection
        public virtual int Count => Instance is IEnumerable enumerable ? enumerable.Count() : 0;
    }
}


