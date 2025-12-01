using System;
using System.Collections.Generic;
using Utility.Structs.Repos;

namespace Utility.Interfaces.Exs
{
    public interface ITreeRepository
    {
        IObservable<Key?> InsertRoot(Guid guid, string name, Type type);

        IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null);

        IObservable<Utility.Changes.Change<Key>> Find(Guid? parentGuid = default, string? name = null, Guid? guid = null, Type? type = null, int? index = null);

        int? MaxIndex(Guid parentGuid, string? name = null);

        DateTime Remove(Guid guid);

        void UpdateName(Guid parentGuid, Guid guid, string name, string newName);

        void Reset();

        IObservable<Key> FindRecursive(Guid parentGuid, int? maxIndex = null);
    }

    public interface IValueRepository
    {
        void Copy(Guid guid, Guid newGuid);
        IObservable<DateValue> Get(Guid guid, string? name = null);
        void Set(Guid guid, string name, object value, DateTime dateTime);
    }
}