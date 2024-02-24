using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Utility.Descriptors
{
    public class PropertyDescriptorComparer : IEqualityComparer<PropertyDescriptor>
    {
        public bool Equals(PropertyDescriptor? x, PropertyDescriptor? y)
        {
            return x?.Name == y?.Name && x?.ComponentType.Name == y?.ComponentType.Name;
        }

        public int GetHashCode([DisallowNull] PropertyDescriptor obj)
        {
            return obj.GetHashCode();
        }
    }
}
