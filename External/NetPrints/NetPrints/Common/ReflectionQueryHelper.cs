using NetPrints.Core;
using NetPrints.Graph;
using NetPrints.Interfaces;
using Splat;
using System.Linq;

namespace NetPrintsEditor.Reflection
{

    public static class ReflectionQueryHelper
    {

        public static T WithVisibleFrom<T>(this T query, ITypeSpecifier visibleFrom) where T : IReflectionProviderQuery
        {
            if (visibleFrom != null)
                query.VisibleFrom = visibleFrom;
            return query;
        }

        public static T WithStatic<T>(this T query, bool? isStatic) where T : IReflectionProviderQuery
        {
            if (isStatic.HasValue)
                query.Static = isStatic;
            return query;
        }

        public static T WithNameLike<T>(this T query, string regexString) where T : IReflectionProviderQuery
        {
            if (string.IsNullOrEmpty(regexString) == false && regexString.Length > 0)
                query.NameLike = regexString;
            return query;
        }

        public static T WithType<T>(this T query, ITypeSpecifier type) where T : IType
        {
            if (type != null)
                query.Type = type;
            return query;
        }

        public static IReflectionProviderMethodQuery WithReturnType(this IReflectionProviderMethodQuery query, ITypeSpecifier returnType)
        {
            if (returnType != null)
                query.ReturnType = returnType;
            return query;
        }


        public static IReflectionProviderMethodQuery WithArgumentType(this IReflectionProviderMethodQuery query, ITypeSpecifier argumentType)
        {
            if (argumentType != null)
                query.ArgumentType = argumentType;
            return query;
        }

        public static IReflectionProviderMethodQuery WithHasGenericArguments(this IReflectionProviderMethodQuery query, bool? hasGenericArguments)
        {
            if (hasGenericArguments.HasValue)
                query.HasGenericArguments = hasGenericArguments;
            return query;
        }

        public static IReflectionProviderVariableQuery WithVariableType(this IReflectionProviderVariableQuery query, ITypeSpecifier typeSpecifier, bool derivesFrom = false)
        {
            query.VariableType = typeSpecifier;
            query.VariableTypeDerivesFrom = derivesFrom;
            return query;
        }

    }

}
