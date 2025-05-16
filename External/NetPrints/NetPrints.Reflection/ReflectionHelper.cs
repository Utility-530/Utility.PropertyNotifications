using Microsoft.CodeAnalysis.CSharp.Syntax;
using NetPrints.Core;
using NetPrints.Graph;
using NetPrints.Interfaces;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetPrintsEditor.Reflection
{


    public class ReflectionProviderMethodQuery : IReflectionProviderMethodQuery, IEqualityComparer<ReflectionProviderMethodQuery>
    {
        public ITypeSpecifier Type { get; set; }
        public bool? Static { get; set; }
        public ITypeSpecifier VisibleFrom { get; set; }
        public ITypeSpecifier ReturnType { get; set; }
        public ITypeSpecifier ArgumentType { get; set; }
        public bool? HasGenericArguments { get; set; }

        public bool Equals(ReflectionProviderMethodQuery x, ReflectionProviderMethodQuery y)
        {
            return x.Type == y.Type && x.Static == y.Static && x.VisibleFrom == y.VisibleFrom
                && x.ReturnType == y.ReturnType && x.ArgumentType == y.ArgumentType && x.HasGenericArguments == y.HasGenericArguments;
        }

        public int GetHashCode(ReflectionProviderMethodQuery obj)
        {
            return HashCode.Combine(Type, Static, VisibleFrom, ReturnType, ArgumentType, HasGenericArguments);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj)
                || (obj is ReflectionProviderMethodQuery query && Equals(this, query));
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
    }


    public static class ReflectionHelper
    {

        public static T WithVisibleFrom<T>(this T query, ITypeSpecifier visibleFrom) where T: IReflectionProviderQuery
        {
            query.VisibleFrom = visibleFrom;
            return query;
        }

        public static T WithStatic<T>(this T query, bool isStatic) where T : IReflectionProviderQuery
        {
            query.Static = isStatic;
            return query;
        }

    }


    public static class ReflectionMethodHelper
    {

        public static IReflectionProviderMethodQuery WithType(this IReflectionProviderMethodQuery query, ITypeSpecifier type)
        {
            query.Type = type;
            return query;
        }



        public static IReflectionProviderMethodQuery WithReturnType(this IReflectionProviderMethodQuery query, ITypeSpecifier returnType)
        {
            query.ReturnType = returnType;
            return query;
        }

        public static IReflectionProviderMethodQuery WithArgumentType(this IReflectionProviderMethodQuery query, ITypeSpecifier argumentType)
        {
            query.ArgumentType = argumentType;
            return query;
        }

        public static IReflectionProviderMethodQuery WithHasGenericArguments(this IReflectionProviderMethodQuery query, bool hasGenericArguments)
        {
            query.HasGenericArguments = hasGenericArguments;
            return query;
        }
    }

    public static class ReflectionVariableHelper
    {
        public static IReflectionProviderVariableQuery WithType(this IReflectionProviderVariableQuery query, ITypeSpecifier type)
        {
            query.Type = type;
            return query;
        }

        //public static IReflectionProviderVariableQuery WithStatic(this IReflectionProviderVariableQuery query, bool isStatic)
        //{
        //    query.Static = isStatic;
        //    return query;
        //}

        public static IReflectionProviderVariableQuery WithVisibleFrom(this IReflectionProviderVariableQuery query, ITypeSpecifier visibleFrom)
        {
            query.VisibleFrom = visibleFrom;
            return query;
        }

        public static IReflectionProviderVariableQuery WithVariableType(this IReflectionProviderVariableQuery query, ITypeSpecifier typeSpecifier, bool derivesFrom = false)
        {
            query.VariableType = typeSpecifier;
            query.VariableTypeDerivesFrom = derivesFrom;
            return query;
        }

    }


    public class ReflectionProviderVariableQuery : IReflectionProviderVariableQuery, IEqualityComparer<ReflectionProviderVariableQuery>
    {
        public bool? Static { get; set; }
        public ITypeSpecifier Type { get; set; }
        public ITypeSpecifier VisibleFrom { get; set; }
        public ITypeSpecifier VariableType { get; set; }
        public bool VariableTypeDerivesFrom { get; set; } = false;

        public ReflectionProviderVariableQuery WithType(ITypeSpecifier type)
        {
            Type = type;
            return this;
        }

        public ReflectionProviderVariableQuery WithStatic(bool isStatic)
        {
            Static = isStatic;
            return this;
        }

        public ReflectionProviderVariableQuery WithVisibleFrom(ITypeSpecifier visibleFrom)
        {
            VisibleFrom = visibleFrom;
            return this;
        }

        public ReflectionProviderVariableQuery WithVariableType(ITypeSpecifier type, bool derivesFrom = false)
        {
            VariableType = type;
            VariableTypeDerivesFrom = derivesFrom;
            return this;
        }

        public bool Equals(ReflectionProviderVariableQuery x, ReflectionProviderVariableQuery y)
        {
            return x.Type == y.Type && x.Static == y.Static && x.VisibleFrom == y.VisibleFrom
                && x.VariableType == y.VariableType && x.VariableTypeDerivesFrom == y.VariableTypeDerivesFrom;
        }

        public int GetHashCode(ReflectionProviderVariableQuery obj)
        {
            return HashCode.Combine(Type, Static, VisibleFrom, VariableType, VariableTypeDerivesFrom);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj)
                || (obj is ReflectionProviderVariableQuery query && Equals(this, query));
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }
    }

}
