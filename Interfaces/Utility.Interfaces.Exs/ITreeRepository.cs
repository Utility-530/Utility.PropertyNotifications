using System;
using System.Collections.Generic;
using Utility.Structs.Repos;

namespace Utility.Interfaces.Exs
{
    public interface ITreeRepository
    {
        void Copy(Guid guid, Guid newGuid);
        IObservable<Key?> InsertRoot(Guid guid, string name, Type type);
        IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null);
        IObservable<Key?> Find(Guid parentGuid, string? name = null, Guid? guid = null, Type? type = null, int? index = null);
        IObservable<DateValue> Get(Guid guid, string? name = null);
        IObservable<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null);
        int? MaxIndex(Guid parentGuid, string? name = null);
        DateTime Remove(Guid guid);
        IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null);
        void Set(Guid guid, string name, object value, DateTime dateTime);
        void UpdateName(Guid parentGuid, Guid guid, string name, string newName);

        void Reset();
        IObservable<Key> FindRecursive(Guid parentGuid, int? maxIndex = null);
    }
}