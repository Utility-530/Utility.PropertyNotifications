using System;
using System.Collections.Generic;
using System.Text;

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
