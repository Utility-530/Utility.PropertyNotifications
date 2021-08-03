using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityStruct
{
    public ref struct Name
    {
        public ReadOnlySpan<char> Value { get; }

        public Name(ReadOnlySpan<char> value) : this(value, 0)
        {
        }

        public Name(ReadOnlySpan<char> value, int position)
        {
            Value = value;
            Order = position;
        }
        public int Order { get; }
    }
}
