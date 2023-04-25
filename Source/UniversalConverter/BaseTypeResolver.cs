using System;

namespace UniversalConverter
{
    public class BaseTypeResolver
    {
        public static Type? ResolveType(string fullName, bool throwOnError)
        {
            if (fullName == null)
                throw new ArgumentNullException("fullName");

            var type = Type.GetType(fullName, throwOnError);
            return type;
        }
    }
}