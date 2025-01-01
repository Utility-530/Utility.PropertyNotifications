namespace Utility.Repos
{
    public interface ITreeRepository
    {
        void Copy(Guid guid, Guid newGuid);
        IObservable<Key> InsertRoot(Guid guid, string name, Type type);
        IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null);
        IObservable<Key> Find(Guid parentGuid, string name, Type? type = null, int? index = null);
        IObservable<DateValue> Get(Guid guid);
        //int Insert(Guid guid, string name, Type type, Guid parentGuid, int? index );
        IObservable<Guid> InsertByParent(Guid parentGuid, string name, string? table_name = null, int? typeId = null, int? index = null);
        int? MaxIndex(Guid parentGuid, string? name = null);
        //Task<IReadOnlyCollection<Key>> Keys();
        //void Register(Guid guid, INotifyPropertyCalled propertyCalled);
        //void Register(Guid guid, INotifyPropertyReceived propertyReceived);
        void Remove(Guid guid);
        IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null);
        void Set(Guid guid, object value, DateTime dateTime);
    }
}