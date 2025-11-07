namespace Utility.Keys
{
    public record GuidKey : ValueKey<Guid>
    {
        public GuidKey(Guid? value = default) : base(value ?? Guid.NewGuid())
        {
        }

        public static explicit operator GuidKey(string b) => new(Guid.Parse(b));

        public static explicit operator string(GuidKey b) => b.ToString();

        public static implicit operator Guid(GuidKey b) => b.Value;

        public static explicit operator GuidKey(Guid b) => new(b);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}