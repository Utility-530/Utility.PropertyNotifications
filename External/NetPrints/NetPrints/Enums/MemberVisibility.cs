//using NetPrints.Graph;
using System;

namespace NetPrints.Enums
{
    /// <summary>
    /// Visibility for methods, properties and other members.
    /// </summary>
    [Flags]
    public enum MemberVisibility
    {
        Invalid = 0,
        Private = 1,
        Public = 2,
        Protected = 4,
        Internal = 8,

        Any = Private | Public | Protected | Internal,
        ProtectedOrPublic = Protected | Public,
    }
}
