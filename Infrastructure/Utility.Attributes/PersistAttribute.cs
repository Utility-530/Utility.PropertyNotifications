using System;

namespace Utility.Attributes
{
    public class PersistAttribute(bool should) : Attribute
    {
        public bool Should { get; } = should;
    }
}