using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Commands;

namespace Utility.Nodify.Views.Infrastructure
{
    internal class MessageBoxCommand : Command
    {
        public MessageBoxCommand() : base(() => { }, ()=>true)
        {
        }

        public static MessageBoxCommand Instance { get; } = new MessageBoxCommand();
    }
}
