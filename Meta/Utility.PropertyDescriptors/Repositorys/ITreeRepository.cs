namespace Utility.Descriptors.Repositorys
{
    public interface ITreeRepository
    {
        void Copy(Guid guid, Guid newGuid);
        Task<Key> InsertRoot(Guid guid, string name, Type type);
        IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null);
        Task<Guid> Find(Guid parentGuid, string name, Type? type = null, int? index = null);
        DateValue? Get(Guid guid);
        //int Insert(Guid guid, string name, Type type, Guid parentGuid, int? index );
        Task<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null);
        int? MaxIndex(Guid parentGuid, string? name = null);
        //Task<IReadOnlyCollection<Key>> Keys();
        //void Register(Guid guid, INotifyPropertyCalled propertyCalled);
        //void Register(Guid guid, INotifyPropertyReceived propertyReceived);
        void Remove(Guid guid);
        Task<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null);
        void Set(Guid guid, object value, DateTime dateTime);
    }
}