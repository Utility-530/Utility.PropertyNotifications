using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Utility.Roslyn
{
    public static class SpecialTypeHelper
    {
        public static SpecialType ToSpecialType(this Type type)
        {
            if (type == null)
                return SpecialType.None;

            // unwrap Nullable<T>
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(bool)) return SpecialType.System_Boolean;
            if (type == typeof(byte)) return SpecialType.System_Byte;
            if (type == typeof(sbyte)) return SpecialType.System_SByte;
            if (type == typeof(short)) return SpecialType.System_Int16;
            if (type == typeof(ushort)) return SpecialType.System_UInt16;
            if (type == typeof(int)) return SpecialType.System_Int32;
            if (type == typeof(uint)) return SpecialType.System_UInt32;
            if (type == typeof(long)) return SpecialType.System_Int64;
            if (type == typeof(ulong)) return SpecialType.System_UInt64;
            if (type == typeof(float)) return SpecialType.System_Single;
            if (type == typeof(double)) return SpecialType.System_Double;
            if (type == typeof(decimal)) return SpecialType.System_Decimal;
            if (type == typeof(char)) return SpecialType.System_Char;
            if (type == typeof(string)) return SpecialType.System_String;
            if (type == typeof(object)) return SpecialType.System_Object;
            if (type == typeof(void)) return SpecialType.System_Void;

            return SpecialType.None;
        }
    }
}
