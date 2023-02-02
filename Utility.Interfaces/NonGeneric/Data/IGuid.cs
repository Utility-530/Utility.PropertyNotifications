using System;

namespace Utility.Interfaces.NonGeneric.Data
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
