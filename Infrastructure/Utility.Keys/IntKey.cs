namespace Utility.Keys
{
    public record IntKey : ValueKey<int>
    {
        public IntKey(int value) : base(value)
        {
        }
    }
}