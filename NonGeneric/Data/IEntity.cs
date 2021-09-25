using System;

namespace UtilityInterface.NonGeneric.Data
{
    public interface IEntity : IId, IIdSet, IGuid, IGuidSet, IEquatable<IEntity>
    {
        DateTime Addition { get; set; }
        DateTime LastUpdate { get; set; }

    }
}
