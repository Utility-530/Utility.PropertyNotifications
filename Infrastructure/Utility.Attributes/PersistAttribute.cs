using System;

namespace Utility.Attributes
{
    public class PersistAttribute(bool shouldPersist) : Attribute
    {
        public bool ShouldPersist { get; } = shouldPersist;
    }
}