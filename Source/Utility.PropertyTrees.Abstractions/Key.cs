using Utility.PropertyTrees.Abstractions;

namespace Utility.PropertyTrees.Abstractions
{
    public record Key(Guid Guid, string Name, Type Type) : IKey
    {
        public bool Equals(IKey? other)
        {
            return Equals(other as Key);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}