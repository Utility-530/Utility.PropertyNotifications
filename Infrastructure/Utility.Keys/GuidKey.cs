namespace Utility.Keys
{
    public class GuidKey : ValueKey<Guid>
    {
        public GuidKey(Guid? value = default) : base(value?? Guid.NewGuid())
        {
            if(Value== default(Guid))
            {

            }
        }
    }
}