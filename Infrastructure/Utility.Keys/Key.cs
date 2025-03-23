

namespace Utility.Keys
{
    public record Key 
    {
        public static implicit operator string?(Key d) => d?.ToString();
    }
}