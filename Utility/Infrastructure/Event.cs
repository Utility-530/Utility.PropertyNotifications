using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Infrastructure
{
    public record Event(string Key);

    public record IsCompleteEvent(string Key, bool? Value) : Event(Key);

    public record CloseRequest(string Key, bool Value) : Event(Key);

}
