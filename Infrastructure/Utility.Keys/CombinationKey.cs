using Utility.Interfaces.NonGeneric;

namespace Utility.Keys
{
    public class CombinationKey : IEquatable
    {
        public CombinationKey(IEquatable[] equatables)
        {
            Equatables = equatables;
        }

        public IEquatable[] Equatables { get; }

        public bool Equals(IEquatable? other)
        {
            return Equals(other as CombinationKey);
        }

        public bool Equals(CombinationKey? other)
        {
            return this.Equatables.Join(other?.Equatables ?? Array.Empty<IEquatable>(), a => a, a => a, (a, b) => a.Equals(b)).All(a => a);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CombinationKey);
        }
    }
}