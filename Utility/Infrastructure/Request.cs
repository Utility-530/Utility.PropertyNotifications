using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Infrastructure
{
    public record Request(string Key);

    public record TaskChangeRequest(string Key, RunningState State) : Request(Key);
}
