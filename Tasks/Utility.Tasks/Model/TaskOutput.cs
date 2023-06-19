using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Tasks.Model
{
    public record TaskOutput : ITaskOutput
    {
        public TaskOutput(string key, object value, bool isCancelled = false)
        {
            Key = key;
            this.Value = value;
            IsCancelled = isCancelled;
        }

        public string Key { get; }

        public virtual object Value { get; }

        public bool IsCancelled { get; }

        public bool Equals(IKey<string> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable other)
        {
            throw new NotImplementedException();
        }
    }

    public record StringTaskOutput : TaskOutput
    {
        public StringTaskOutput(string key, string value, bool isCancelled =false) : base(key, value, isCancelled)
        {
            Value = value;
        }

        public override string Value { get; }
    }

}
