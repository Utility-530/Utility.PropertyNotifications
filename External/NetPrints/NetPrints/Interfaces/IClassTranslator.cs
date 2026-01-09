using System;
using System.Collections.Generic;
using System.Text;

namespace NetPrints.Interfaces
{
    public interface IClassTranslator
    {
        string TranslateClass(IClassGraph cls);
    }
}
