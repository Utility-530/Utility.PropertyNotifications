using NetPrints.Interfaces;
using System;
using System.Collections.Generic;

namespace NetPrintsEditor.Reflection
{
    public class ReflectionProviderVariableQuery : IReflectionProviderVariableQuery, IEqualityComparer<ReflectionProviderVariableQuery>
    {
        public bool? Static { get; set; }
        public ITypeSpecifier Type { get; set; }
        public ITypeSpecifier VisibleFrom { get; set; }
        public ITypeSpecifier VariableType { get; set; }
        public bool VariableTypeDerivesFrom { get; set; } = false;
        public string? NameLike { get; set; }

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
