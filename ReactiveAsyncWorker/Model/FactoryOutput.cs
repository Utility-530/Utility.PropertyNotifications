using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveAsyncWorker.Model
{
    public class FactoryOutput
    {
    }

    public class StringFactoryOutput : FactoryOutput
    {
        public StringFactoryOutput(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
