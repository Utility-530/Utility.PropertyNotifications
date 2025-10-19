using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Attributes
{
    public  class SymbolAttribute(string symbol):Attribute
    {
        public string Symbol { get; } = symbol;
    }
}
