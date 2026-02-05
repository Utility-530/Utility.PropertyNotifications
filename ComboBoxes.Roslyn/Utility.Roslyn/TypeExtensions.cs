using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Utility.Roslyn
{
    public static class TypeExtensions
    {
        public static bool IsEquivalent(this Type runtimeType, ITypeSymbol symbol)
        {
            if (symbol == null) return false;

            // Handle nullable
            if (runtimeType.IsGenericType && runtimeType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (symbol is INamedTypeSymbol named && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
                    return IsEquivalent(runtimeType.GetGenericArguments()[0], named.TypeArguments[0]);
                return false;
            }

            // Arrays
            if (runtimeType.IsArray)
            {
                if (symbol is IArrayTypeSymbol arrSymbol)
                    return IsEquivalent(runtimeType.GetElementType()!, arrSymbol.ElementType) &&
                           arrSymbol.Rank == runtimeType.GetArrayRank();
                return false;
            }

            // Use SpecialType for primitives
            var runtimeSpecial = runtimeType switch
            {
                var t when t == typeof(bool) => SpecialType.System_Boolean,
                var t when t == typeof(byte) => SpecialType.System_Byte,
                var t when t == typeof(sbyte) => SpecialType.System_SByte,
                var t when t == typeof(short) => SpecialType.System_Int16,
                var t when t == typeof(ushort) => SpecialType.System_UInt16,
                var t when t == typeof(int) => SpecialType.System_Int32,
                var t when t == typeof(uint) => SpecialType.System_UInt32,
                var t when t == typeof(long) => SpecialType.System_Int64,
                var t when t == typeof(ulong) => SpecialType.System_UInt64,
                var t when t == typeof(float) => SpecialType.System_Single,
                var t when t == typeof(double) => SpecialType.System_Double,
                var t when t == typeof(decimal) => SpecialType.System_Decimal,
                var t when t == typeof(char) => SpecialType.System_Char,
                var t when t == typeof(string) => SpecialType.System_String,
                var t when t == typeof(object) => SpecialType.System_Object,
                var t when t == typeof(void) => SpecialType.System_Void,
                _ => SpecialType.None
            };

            if (runtimeSpecial != SpecialType.None)
                return symbol.SpecialType == runtimeSpecial;

            // Fallback for framework types like DateTime, Guid
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                         .Replace("global::", "")
                   == runtimeType.FullName;
        }
        public static ITypeSymbol? ToTypeSymbol(this Type type, Compilation compilation)
        {
            if (type == null)
                return null;

            // Handle nullable<T>
            if (Nullable.GetUnderlyingType(type) is Type underlying)
            {
                var underlyingSymbol = ToTypeSymbol(underlying, compilation);
                if (underlyingSymbol == null)
                    return null;

                return compilation.GetSpecialType(SpecialType.System_Nullable_T)
                    is INamedTypeSymbol nullable
                        ? nullable.Construct(underlyingSymbol)
                        : null;
            }

            // Arrays
            if (type.IsArray)
            {
                var elementSymbol = ToTypeSymbol(type.GetElementType()!, compilation);
                return elementSymbol == null
                    ? null
                    : compilation.CreateArrayTypeSymbol(
                        elementSymbol,
                        type.GetArrayRank());
            }

            // Non-generic / generic type definitions
            var metadataName = GetMetadataName(type);
            var symbol = compilation.GetTypeByMetadataName(metadataName);

            if (symbol == null)
                return null;

            // Construct generic types
            if (type.IsGenericType && symbol is INamedTypeSymbol named)
            {
                var typeArguments = type.GetGenericArguments()
                    .Select(t => ToTypeSymbol(t, compilation))
                    .ToArray();

                if (typeArguments.Any(t => t == null))
                    return null;

                return named.Construct(typeArguments!);
            }

            return symbol;
        }
      
        //    Metadata name helper(important!)
        //GetTypeByMetadataName requires CLR metadata names, not C# names.

        private static string GetMetadataName(Type type)
        {
            if (type.IsNested)
                return $"{GetMetadataName(type.DeclaringType!)}+{type.Name}";

            if (!type.IsGenericType)
                return type.FullName!;

            var genericDefinition = type.GetGenericTypeDefinition();
            return genericDefinition.FullName!;
        }
    }
}