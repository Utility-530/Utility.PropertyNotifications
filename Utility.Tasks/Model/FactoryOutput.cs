using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Tasks.Model
{
    public class FactoryOutput
    {
        public FactoryOutput(object value)
        {
            Value = value;
        }

        public virtual object Value { get; }
    }

    public class StringFactoryOutput : FactoryOutput
    {
        public StringFactoryOutput(string value) : base(value)
        {
            Value = value;
        }

        public override string Value { get; }
    }
}
