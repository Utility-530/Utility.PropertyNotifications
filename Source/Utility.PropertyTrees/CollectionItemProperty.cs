using Utility.Conversions;
using Utility.PropertyTrees.Infrastructure;

namespace Utility.PropertyTrees
{
    public class CollectionItemValueProperty : ValueProperty
    {
        public CollectionItemValueProperty(Guid guid) : base(guid)
        {
        }

        public int Index => (Descriptor as CollectionItemDescriptor)?.Index ?? throw new Exception("vdfsss3 3333");
    }
    public class CollectionItemReferenceProperty : ReferenceProperty
    {
        public CollectionItemReferenceProperty(Guid guid) : base(guid)
        {
        }

        public int Index => (Descriptor as CollectionItemDescriptor)?.Index ?? throw new Exception("vdfsss3 3333");       
    }
}