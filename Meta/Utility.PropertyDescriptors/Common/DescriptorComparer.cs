namespace Utility.PropertyDescriptors
{
    public class DescriptorComparer : IEqualityComparer<IDescriptor>
    {
        public bool Equals(IDescriptor? x, IDescriptor? y)
        {
            return x?.Name == y?.Name && x?.ParentType.Name == y?.ParentType.Name;
        }

        public int GetHashCode([DisallowNull] IDescriptor obj)
        {
            return obj.GetHashCode();
        }
    }
}