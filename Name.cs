using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityStruct
{
    public class Name
    {
        public string Value { get; }

        public Name(string value): this(value,0)
        {
        }

        public Name(string value, int position)
        {
            Value = value;
            Order = position;
        }
        public int Order { get; }
    }
}
