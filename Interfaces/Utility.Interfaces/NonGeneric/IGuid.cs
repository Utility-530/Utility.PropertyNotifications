using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IGuid
    {
        Guid Guid { get; }// = Guid.Parse("901f3c6d-1424-4771-8672-0b77d7c44342");
    }

    public interface IGuidSet
    {
        Guid Guid { set; }
    }
}