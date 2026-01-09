using System;
using System.Collections.Generic;
using System.Text;

namespace NetPrints.Core.Common
{
    public class TypeSpecifierUtil
    {
        /// <summary>
        /// Translates an object into a literal value (eg. a float 32.32 -> "32.32f")
        /// </summary>
        /// <param name="obj">Object value to translate.</param>
        /// <param name="type">Specifier for the type of the literal.</param>
        /// <returns></returns>
        public static string ObjectToLiteral(object obj, TypeSpecifier type)
        {
            // Interpret object string as enum field
            if (type.IsEnum)
            {
                return $"{type}.{obj}";
            }

            // Null
            if (obj is null)
            {
                return "null";
            }

            // Put quotes around string literals
            if (type == TypeSpecifier.FromType<string>())
            {
                return $"\"{obj}\"";
            }
            else if (type == TypeSpecifier.FromType<float>())
            {
                return $"{obj}F";
            }
            else if (type == TypeSpecifier.FromType<double>())
            {
                return $"{obj}D";
            }
            else if (type == TypeSpecifier.FromType<uint>())
            {
                return $"{obj}U";
            }
            // Put single quotes around char literals
            else if (type == TypeSpecifier.FromType<char>())
            {
                return $"'{obj}'";
            }
            else if (type == TypeSpecifier.FromType<long>())
            {
                return $"{obj}L";
            }
            else if (type == TypeSpecifier.FromType<ulong>())
            {
                return $"{obj}UL";
            }
            else if (type == TypeSpecifier.FromType<decimal>())
            {
                return $"{obj}M";
            }
            else
            {
                return obj.ToString();
            }
        }
    }
}
