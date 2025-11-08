using System;

namespace Utility.Structs
{
    public readonly ref struct Name
    {
        public ReadOnlySpan<char> Value { get; }

        public Name(ReadOnlySpan<char> value) : this(value, 0)
        {
        }

        public Name(ReadOnlySpan<char> value, int position)
        {
            this.Value = value;
            this.Order = position;
        }

        public int Order { get; }
    }
}