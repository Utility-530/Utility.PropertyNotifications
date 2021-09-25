using System;

namespace UtilityInterface.NonGeneric.Data
{
    public interface IGuid
    {
        Guid Guid { get; }
    }

    public interface IGuidSet
    {
        Guid Guid { set; }
    }
}
