using System;
using ReactiveUI;
using Utility.Helpers.Ex;
using Utility.Interfaces.Generic.Data;

namespace Utility.Common.Model
{
    public class GuidModel<T> : ReactiveObject, IEquatable<T>, IId<Guid> where T : IId<Guid>
    {
        public Guid Id { get; set; }

        public bool Equals(T? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((T)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return TextJsonHelper.Serialise(this, this.GetType());
        }
    }
}
