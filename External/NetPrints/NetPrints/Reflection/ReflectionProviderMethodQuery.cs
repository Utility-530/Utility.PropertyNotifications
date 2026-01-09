using NetPrints.Interfaces;
using System;
using System.Collections.Generic;

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

        public string? NameLike { get; set; }

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

}
