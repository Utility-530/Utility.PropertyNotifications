using Utility.Conversions;
using Utility.PropertyTrees.Infrastructure;

namespace Utility.PropertyTrees
{

    public interface ICollectionItemProperty
    {
       public Type Type { get; }
    }

    public class CollectionItemValueProperty : ValueProperty, ICollectionItemProperty
    {
        public CollectionItemValueProperty(Guid guid) : base(guid)
        {
        }

        public int Index => (Descriptor as CollectionItemDescriptor)?.Index ?? throw new Exception("vdfsss3 3333");

        Type ICollectionItemProperty.Type => PropertyType;
    }
    public class CollectionItemReferenceProperty : ReferenceProperty, ICollectionItemProperty
    {
        public CollectionItemReferenceProperty(Guid guid) : base(guid)
        {
        }

        public int Index => (Descriptor as CollectionItemDescriptor)?.Index ?? throw new Exception("vdfsss3 3333");

        Type ICollectionItemProperty.Type => PropertyType;
    }
}