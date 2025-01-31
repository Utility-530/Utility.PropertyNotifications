using System;

namespace Utility.Interfaces.NonGeneric.Data
{
    public interface IEntity : IId, IIdSet, IGetGuid, ISetGuid, IEquatable<IEntity>
    {
        DateTime Addition { get; set; }
        DateTime LastUpdate { get; set; }

    }
}
