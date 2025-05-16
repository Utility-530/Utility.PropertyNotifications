//using NetPrints.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Enums
{
    /// <summary>
    /// Modifiers a method can have. Can be combined.
    /// </summary>
    [Flags]
    public enum MethodModifiers
    {
        None = 0,
        Sealed = 8,
        Abstract = 16,
        Static = 32,
        Virtual = 64,
        Override = 128,
        Async = 256,

        // DEPRECATED
        // Moved to MethodVisibility
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
