using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Models;

namespace Utility.Infrastructure
{
    public record TaskChangeRequest(string Key, RunningState State) : Request();
}
