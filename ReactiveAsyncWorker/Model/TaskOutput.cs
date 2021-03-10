using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveAsyncWorker.Model
{
    public class TaskOutput
    {
    }

    public class StringTaskOutput: TaskOutput
    {
        public StringTaskOutput(string value)
        {
            this.Value = value;
        }

        public string Value { get; }
    }

}
