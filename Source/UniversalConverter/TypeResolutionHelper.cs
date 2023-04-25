using SoftFluent.Windows;
using System;

namespace UniversalConverter
{
    public static class TypeResolutionHelper
    {
        public static Type? ResolveType(string fullName)
        {
            return ResolveType(fullName, false);
        }

        public static Type? ResolveType(string fullName, bool throwOnError)
        {
            return BaseTypeResolver.ResolveType(fullName, throwOnError) ?? throw new Exception("dsv fsd");
        }
    }
}