using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.NonGeneric.Database
{
    public interface IDbEntity : IId, ISetId, IGuid, ISetGuid, IEquatable<IDbEntity>
    {
        DateTime AddedTime { get; set; }

    }
}
