using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Keys
{
    public record TypeNameKey(Type Type, string Name) : Key
    {
    }
}
