//using NetPrints.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Enums
{
    /// <summary>
    /// Modifiers a class can have. Can be combined.
    /// </summary>
    [Flags]
    public enum ClassModifiers
    {
        None = 0,
        Sealed = 8,
        Abstract = 16,
        Static = 32,
        Partial = 64,

        // Deprecated
        [Obsolete]
        Private = 0,
        [Obsolete]
        Public = 1,
        [Obsolete]
        Protected = 2,
        [Obsolete]
        Internal = 4,
    }
}
