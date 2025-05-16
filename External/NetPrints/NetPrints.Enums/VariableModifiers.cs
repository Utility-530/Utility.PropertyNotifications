using System;

namespace NetPrints.Enums
{
    /// <summary>
    /// Modifiers variables can have. Can be combined.
    /// </summary>
    [Flags]
    public enum VariableModifiers
    {
        None = 0,
        ReadOnly = 8,
        Const = 16,
        Static = 32,
        New = 64,

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
