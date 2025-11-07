using System;

namespace Utility.Attributes
{
    public class SymbolAttribute(string symbol) : Attribute
    {
        public string Symbol { get; } = symbol;
    }
}