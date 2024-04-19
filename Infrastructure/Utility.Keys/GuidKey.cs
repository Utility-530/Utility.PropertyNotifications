namespace Utility.Keys
{
    public record GuidKey : ValueKey<Guid>
    {
        public GuidKey(Guid? value = default) : base(value?? Guid.NewGuid())
        {
            if(Value== default(Guid))
            {

            }
        }

        public static explicit operator GuidKey(string b) => new GuidKey(Guid.Parse(b));


        public override string ToString()
        {
            return Value.ToString();
        }

    }
}