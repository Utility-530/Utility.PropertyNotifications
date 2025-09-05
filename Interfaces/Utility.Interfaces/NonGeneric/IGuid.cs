using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IGuid : IGetGuid, ISetGuid
    {
    }

    public interface IGetGuid
    {
        Guid Guid { get; }
    }

    public interface ISetGuid
    {
        Guid Guid { set; }
    }
}