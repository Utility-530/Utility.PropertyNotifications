using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IPersist
    {
        void Save(Guid key);

        void Load(Guid key);
    }
}
