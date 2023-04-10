using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public interface ISerialise
    {
        string ToString();

        ISerialise FromString(string str);

    }
}
