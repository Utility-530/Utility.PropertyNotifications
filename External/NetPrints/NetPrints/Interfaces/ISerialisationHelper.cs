using System;
using System.Collections.Generic;
using System.Text;

namespace NetPrints.Interfaces
{
    public interface ISerialisationHelper
    {
        IClassGraph LoadClass(string v);
    }
}
